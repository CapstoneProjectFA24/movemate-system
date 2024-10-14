using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.IServices;
using MoveMate.Service.Commons;
using MoveMate.Service.Library;
using MoveMate.Service.Exceptions;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;


namespace MoveMate.Service.ThirdPartyService.Payment.VNPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly IUserServices _userService;
        private readonly IWalletServices _walletService;
        private readonly IBookingServices _bookingServices;
        private readonly UnitOfWork _unitOfWork;
        private const string DefaultPaymentInfo = "Thanh toán với VnPay";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly VnPaySettings _vnPaySettings;

        public VnPayService(IConfiguration config, IUserServices userService, IBookingServices bookingServices, IWalletServices walletService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, VnPaySettings vnPaySettings)
        {
            _config = config;
            _userService = userService;
            _bookingServices = bookingServices;
            _walletService = walletService;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _vnPaySettings = vnPaySettings;
        }


        public async Task<OperationResult<string>> Recharge(HttpContext context, int userId, double amount, string returnUrl)
        {
            var result = new OperationResult<string>();

            try
            {
                var time = DateTime.Now;
                var vnpay = new VnPayLibrary();

                // Retrieve wallet by user ID
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, "Wallet not found");
                    return result;
                }

                var userResult = await _unitOfWork.UserRepository.GetByIdAsync(wallet.UserId);
                if (userResult == null)
                {
                    result.AddError(StatusCode.NotFound, "User not found");
                    return result;
                }

                // Add VnPay request data
                vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
                vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
                vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", time.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
                vnpay.AddRequestData("vnp_IpAddr", Utilss.GetIpAddress(context));
                vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
                vnpay.AddRequestData("vnp_OrderInfo", wallet.Id.ToString());
                vnpay.AddRequestData("vnp_OrderType", PaymentMethod.RECHARGE.ToString());
                vnpay.AddRequestData("vnp_ReturnUrl", $"{_config["VnPay:RechargeBackReturnUrl"]}?returnUrl={returnUrl}");
                vnpay.AddRequestData("vnp_TxnRef", time.Ticks.ToString());

                // Create payment URL
                var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:PaymentEndpoint"], _config["VnPay:HashSecret"]);
                result.AddResponseStatusCode(StatusCode.Ok, "Payment URL generated successfully", paymentUrl);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, "An internal server error occurred");
            }

            return result;
        }

        public async Task<OperationResult<RechagreResponseModel>> RechagreExecute(IQueryCollection collections)
        {
            var result = new OperationResult<RechagreResponseModel>();

            try
            {
                var vnpay = new VnPayLibrary();

                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
                if (!checkSignature)
                {
                    result.AddError(StatusCode.BadRequest, "Invalid payment signature");
                    return result;
                }

                // Extract wallet ID and amount from the response
                var amount = Convert.ToSingle(vnpay.GetResponseData("vnp_Amount")) / 100f;
                var walletId = int.Parse(vnpay.GetResponseData("vnp_OrderInfo"));

                // Retrieve wallet by user ID
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, "Wallet not found");
                    return result;
                }

                // Update Wallet balance
                wallet.Balance += amount;
                var updateResult = await _walletService.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, "Failed to update wallet balance");
                    return result;
                }

                var responseModel = new RechagreResponseModel
                {
                    Success = true,
                    UserId = wallet.Id,
                    BankCode = vnpay.GetResponseData("vnp_BankCode"),
                    BankTranNo = vnpay.GetResponseData("vnp_BankTranNo"),
                    CardType = vnpay.GetResponseData("vnp_CardType"),
                    Amount = amount,
                    Token = vnp_SecureHash,
                    VnPayResponseCode = vnpay.GetResponseData("vnp_ResponseCode")
                };

                result.AddResponseStatusCode(StatusCode.Ok, "Payment executed successfully", responseModel);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, "An internal server error occurred");
            }

            return result;
        }


        public async Task<OperationResult<Transaction>> RechagrePayment(RechagreResponseModel response)
        {
            var result = new OperationResult<Transaction>();
            var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Retrieve wallet by user ID
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(response.UserId);

                if (wallet == null)
                {
                    result.Message = "Wallet not found";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.NotFound,
                        Message = "Wallet not found"
                    });
                    return result;
                }

                var payment = new Transaction
                {
                    WalletId = wallet.Id,
                    Amount = response.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = PaymentMethod.DEPOSIT.ToString(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Resource = response.BankCode,
                    IsDeleted = false,
                    TransactionCode = "DEP" + Utilss.RandomString(7)
                };

                // Save the payment
                await _unitOfWork.TransactionRepository.AddAsync(payment);
                var count = await _unitOfWork.SaveChangesAsync();

                if (count == 0)
                {
                    await transaction.RollbackAsync();
                    result.Message = "Payment failed";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Payment failed"
                    });
                    return result;
                }

                // Update wallet balance
                // wallet.Balance += payment.Amount;
                wallet.UpdatedAt = DateTime.Now;

                await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                var countUpdate = await _unitOfWork.SaveChangesAsync();

                if (countUpdate == 0)
                {
                    await transaction.RollbackAsync();
                    result.Message = "Payment failed during wallet update";
                    result.IsError = true;
                    result.Errors.Add(new Error()
                    {
                        Code = StatusCode.BadRequest,
                        Message = "Payment failed during wallet update"
                    });
                    return result;
                }

                await transaction.CommitAsync();
                result.Payload = payment;
                result.IsError = false;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                result.Message = "An error occurred: " + e.Message;
                result.IsError = true;
                result.Errors.Add(new Error()
                {
                    Code = StatusCode.ServerError,
                    Message = "An error occurred: " + e.Message
                });
            }

            return result;
        }

        public async Task<OperationResult<string>> CreatePaymentAsync(int bookingId, int userId, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            try
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    operationResult.AddError(StatusCode.BadRequest, "Return URL is required");
                    return operationResult;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    operationResult.AddError(StatusCode.NotFound, "User not found");
                    return operationResult;
                }

                var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
                if (booking == null)
                {
                    operationResult.AddError(StatusCode.NotFound, "Booking not found");
                    return operationResult;
                }

                if (booking.Status != BookingEnums.DEPOSITING.ToString() && booking.Status != BookingEnums.COMPLETED.ToString())
                {
                    operationResult.AddError(StatusCode.BadRequest, "Booking status must be either WAITING or COMPLETED");
                    return operationResult;
                }

                int amount;
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    amount = (int)booking.Deposit;       
                }
                else
                {
                    amount = (int)booking.TotalReal;
                }
                var newGuid = Guid.NewGuid();
                var time = DateTime.Now;

                var serverUrl = string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://",
                                              _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent())
                                              ?? throw new Exception("Server URL is not available");

                var pay = new VnPayLibrary();
                pay.AddRequestData("vnp_ReturnUrl", $"{serverUrl}/{_vnPaySettings.CallbackUrl}?returnUrl={returnUrl}");
                pay.AddRequestData("vnp_Version", _vnPaySettings.Version);
                pay.AddRequestData("vnp_Command", _vnPaySettings.Command);
                pay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
                pay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
                pay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
                pay.AddRequestData("vnp_Amount", (amount * 100).ToString());
                pay.AddRequestData("vnp_CreateDate", time.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_IpAddr", UtilitiesExtensions.GetIpAddress());
                pay.AddRequestData("vnp_OrderInfo", "Payment With Vn Pay");
                pay.AddRequestData("vnp_OrderType", "wallet");
                pay.AddRequestData("vnp_TxnRef", newGuid.ToString());

                var paymentUrl = pay.CreateRequestUrl(_vnPaySettings.PaymentEndpoint, _vnPaySettings.HashSecret);

                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }

        

    }
}