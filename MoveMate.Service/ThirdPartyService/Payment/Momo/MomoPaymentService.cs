using Google.Rpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveMate.Domain.Enums;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using MoveMate.Service.ThirdPartyService.Payment.Momo.Models;
using MoveMate.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using static Grpc.Core.Metadata;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.Library;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;
using MoveMate.Domain.Models;

namespace MoveMate.Service.ThirdPartyService.Payment.Momo
{
    public class MomoPaymentService : IMomoPaymentService
    {
        private const string DefaultOrderInfo = MessageConstant.SuccessMessage.MomoPayment;

        private readonly MomoSettings _momoSettings;
        private readonly ILogger<MomoPaymentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IWalletServices _walletService;
        private readonly IMessageProducer _producer;
        private readonly IFirebaseServices _firebaseServices;

        public MomoPaymentService(
            IOptions<MomoSettings> momoSettings,
            ILogger<MomoPaymentService> logger,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IWalletServices walletServices, IMessageProducer producer, IFirebaseServices firebaseServices)
        {
            _momoSettings = momoSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _walletService = walletServices;
            _producer = producer;
            _firebaseServices = firebaseServices;
        }

        public ClaimsPrincipal? CurrentUserPrincipal => _httpContextAccessor.HttpContext?.User;

        public async Task<OperationResult<string>> CreatePaymentWithMomoAsync(int bookingId, int userId,
            string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            // Validate parameters
            if (string.IsNullOrEmpty(returnUrl))
            {
                operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ReturnUrl);
                return operationResult;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return operationResult;
            }

            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.BookingCannotPay);
                return operationResult;
            }

            var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
            var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);

            if (assignmentPorter == null)
            {
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    //go to
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
                         assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString())
                {
                    //go to
                }
                else
                {
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingStatus);
                    return operationResult;
                }
            }
            else
            {
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    //go to
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
                         assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() &&
                         assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
                {
                    //go to
                }
                else
                {
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingStatus);
                    return operationResult;
                }
            }

            int amount = 0;
            if (booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                amount = (int)booking.Deposit;
            }
            else if (booking.Status == BookingEnums.IN_PROGRESS.ToString())
            {
                amount = (int)booking.TotalReal;
            }

            var newGuid = Guid.NewGuid();
            try
            {
                // Create payment request data
                var payment = new MomoPayment
                {
                    Amount = amount,
                    Info = "order",
                    PaymentReferenceId = bookingId + "-" + newGuid,
                    returnUrl = returnUrl,
                    ExtraData = userId.ToString()
                };

                // Generate Momo payment link
                var paymentUrl = await CreatePaymentAsync(payment, bookingId);

                operationResult =
                    OperationResult<string>.Success(paymentUrl, StatusCode.Ok, MessageConstant.SuccessMessage.CreatePaymentLinkSuccess);
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return operationResult;
            }
        }

        public async Task<string> CreatePaymentAsync(MomoPayment payment, int bookingId)
        {
            string category = "";
            if (payment.Info == "wallet")
            {
                category = CategoryEnums.PAYMENT_WALLET.ToString();
            }
            if (payment.Info == "order")
            {
                var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, int.Parse(payment.ExtraData));
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    category = CategoryEnums.DEPOSIT.ToString();
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString())
                {
                    category = CategoryEnums.PAYMENT_TOTAL.ToString();
                }
            }
            var serverUrl = UrlHelper.GetSecureServerUrl(_httpContextAccessor);
            var requestType = "payWithATM";
            var request = new MomoPaymentRequest
            {
                OrderInfo = payment.Info,
                PartnerCode = _momoSettings.PartnerCode,
                IpnUrl = _momoSettings.IpnUrl,
                RedirectUrl = $"{serverUrl}/{_momoSettings.RedirectUrl}?returnUrl={payment.returnUrl}&category={category}",
                Amount = payment.Amount,
                OrderId = payment.PaymentReferenceId,
                ReferenceId = $"{payment.PaymentReferenceId}",
                RequestId = Guid.NewGuid().ToString(),
                RequestType = requestType,
                ExtraData = payment.ExtraData,
                AutoCapture = true,
                Lang = "vi",
                orderExpireTime = 5
            };

            var rawSignature =
                $"accessKey={_momoSettings.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={request.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={requestType}";
            request.Signature = GetSignature(rawSignature, _momoSettings.SecretKey);

            var httpContent =
                new StringContent(JsonSerializerUtils.Serialize(request), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var momoResponse = await httpClient.PostAsync(_momoSettings.PaymentEndpoint, httpContent);
            var responseContent = await momoResponse.Content.ReadAsStringAsync();

            if (momoResponse.IsSuccessStatusCode)
            {
                var momoPaymentResponse = JsonSerializerUtils.Deserialize<MomoPaymentResponse>(responseContent);

                if (momoPaymentResponse != null)
                {
                    return momoPaymentResponse.PayUrl;
                }
            }
            throw new Exception($"[Momo payment] Error: {responseContent}");
        }

        //util
        private static string GetSignature(string text, string key)
        {
            var encoding = new UTF8Encoding();
            var textBytes = encoding.GetBytes(text);
            var keyBytes = encoding.GetBytes(key);

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


        public async Task<OperationResult<string>> AddFundsToWalletAsync(int userId, double amount, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            // Validate parameters
            if (amount <= 0)
            {
                operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AmountGreaterThanZero);
                return operationResult;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return operationResult;
            }

            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
            if (wallet == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                return operationResult;
            }
            if (wallet.IsLocked == true)
            {
                operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.WalletLocked);
                return operationResult;
            }

            var newGuid = Guid.NewGuid();
            try
            {
                // Create payment request data for Momo
                var payment = new MomoPayment
                {
                    Amount = (int)amount,
                    Info = "wallet",
                    PaymentReferenceId = wallet.Id + "-" + newGuid,
                    returnUrl = returnUrl,
                    ExtraData = userId.ToString()
                };

                // Generate Momo payment link
                var paymentUrl = await CreatePaymentAsync(payment, wallet.Id);

                // Add logic to update user's wallet if payment is successful
                operationResult =
                    OperationResult<string>.Success(paymentUrl, StatusCode.Ok, MessageConstant.SuccessMessage.CreatePaymentLinkSuccess);
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return operationResult;
            }
        }


        public async Task<OperationResult<string>> HandleWalletPaymentAsync(HttpContext context,
            MomoPaymentCallbackCommand command)
        {
            var result = new OperationResult<string>();
            int userId = int.Parse(command.ExtraData);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return result;
            }
            

            // Tìm wallet của user
            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
            if (wallet == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                return result;
            }
            if (command.IsSuccess == false)
            {
                var transactionFail = new MoveMate.Domain.Models.Transaction
                {
                    WalletId = wallet.Id,
                    Amount = (float)command.Amount,
                    Status = PaymentEnum.FAIL.ToString(),
                    TransactionType = Domain.Enums.PaymentMethod.RECHARGE.ToString(),
                    TransactionCode = command.TransId.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Momo.ToString(),
                    PaymentMethod = Resource.Momo.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = true
                };

                await _unitOfWork.TransactionRepository.AddAsync(transactionFail);
                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.TransactionCancel, MessageConstant.FailMessage.TransactionCancel);
                return result;
            }
            try
            {
                wallet.Balance += (float)command.Amount;

                var updateResult = await _walletService.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }

                var transaction = new MoveMate.Domain.Models.Transaction
                {
                    WalletId = wallet.Id,
                    Amount = (float)command.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = Domain.Enums.PaymentMethod.RECHARGE.ToString(),
                    TransactionCode = command.TransId.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Momo.ToString(),
                    PaymentMethod = Resource.Momo.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = true
                };

                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateWalletSuccess, MessageConstant.SuccessMessage.Success);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        public async Task<OperationResult<string>> HandleOrderPaymentAsync(HttpContext context,
            MomoPaymentCallbackCommand callback)
        {
            var operationResult = new OperationResult<string>();

            try
            {
               
                var orderIdParts = callback.OrderId.Split('-');
                if (!int.TryParse(orderIdParts[0], out int bookingId))
                {
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidBookingId);
                    return operationResult;
                }

                var userId = int.Parse(callback.ExtraData);
                var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
                if (booking == null)
                {
                    operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                    return operationResult;
                }

                var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);

                if (callback.IsSuccess == false)
                {
                    var paymentFail = new Domain.Models.Payment
                    {
                        BookingId = bookingId,
                        Amount = (double)callback.Amount,
                        Success = callback.IsSuccess,
                        BankCode = Resource.Momo.ToString()
                    };

                    await _unitOfWork.PaymentRepository.AddAsync(paymentFail);
                    await _unitOfWork.SaveChangesAsync();
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.PaymentFail);
                    return operationResult;
                }

                var payment = new Domain.Models.Payment
                {
                    BookingId = bookingId,
                    Amount = (double)callback.Amount,
                    Success = callback.IsSuccess,
                    Date = DateTime.Now,
                    BankCode = Resource.Momo.ToString()
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                string transType = "";
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    transType = Domain.Enums.PaymentMethod.DEPOSIT.ToString();
                    booking.TotalReal = booking.Total - (float)callback.Amount;
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString()) { 
                    transType = Domain.Enums.PaymentMethod.PAYMENT.ToString();
                    booking.TotalReal -= (float)callback.Amount;
                }
                if (callback.IsSuccess == false)
                {
                    var transactionFail = new MoveMate.Domain.Models.Transaction
                    {
                        PaymentId = payment.Id,
                        Amount = (float)callback.Amount,
                        Status = PaymentEnum.FAIL.ToString(),
                        TransactionType = transType,
                        TransactionCode = callback.TransId.ToString(),
                        CreatedAt = DateTime.Now,
                        Resource = Resource.Momo.ToString(),
                        PaymentMethod = Resource.Momo.ToString(),
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                        IsCredit = false
                    };
                    await _unitOfWork.TransactionRepository.AddAsync(transactionFail);
                    await _unitOfWork.SaveChangesAsync();
                    operationResult = OperationResult<string>.Success(callback.returnUrl, StatusCode.Ok, MessageConstant.SuccessMessage.CreatePaymentLinkSuccess);
                    operationResult.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.TransactionCancel, MessageConstant.FailMessage.TransactionCancel);
                    return operationResult;
                }
                var transaction = new MoveMate.Domain.Models.Transaction
                {
                    PaymentId = payment.Id,
                    Amount = (float)callback.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = transType,
                    TransactionCode = callback.TransId.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Momo.ToString(),
                    PaymentMethod = Resource.Momo.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = false
                };


                // New Transaction for RoleId 6 and Wallet.Tier 0
                var userWithRoleId6 = await _unitOfWork.UserRepository.GetUserByRoleIdAsync();
                if (userWithRoleId6 != null)
                {
                    var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userWithRoleId6.Id);
                    if (wallet != null && wallet.Tier == 0)
                    {
                        var additionalTransaction = new MoveMate.Domain.Models.Transaction
                        {
                            PaymentId = payment.Id,
                            WalletId = wallet.Id,
                            Amount = (double?)callback.Amount,
                            Status = PaymentEnum.SUCCESS.ToString(),
                            TransactionType = Domain.Enums.PaymentMethod.RECEIVE.ToString(),
                            TransactionCode = "R" + Utilss.RandomString(7),
                            CreatedAt = DateTime.Now,
                            Resource = Resource.VNPay.ToString(),
                            PaymentMethod = Resource.VNPay.ToString(),
                            IsDeleted = false,
                            UpdatedAt = DateTime.Now,
                            IsCredit = true
                        };

                        await _unitOfWork.TransactionRepository.AddAsync(additionalTransaction);

                        // Update manager wallet balance
                        wallet.Balance += (float)callback.Amount;

                        var updateResult = await _walletService.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                        if (updateResult.IsError)
                        {
                            operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                            return operationResult;
                        }
                        //await _unitOfWork.SaveChangesAsync();
                    }
                }
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                if (booking.IsReviewOnline == false && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.REVIEWING.ToString();
                    booking.IsDeposited = true;
                }
                else if (booking.IsReviewOnline == true && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.COMING.ToString();
                    booking.IsDeposited = true;
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() && booking.IsDeposited == true)
                {
                    booking.Status = BookingEnums.COMPLETED.ToString();
                }
                else
                {
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                    return operationResult;
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var updatedBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId, includeProperties:
                "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                await _firebaseServices.SaveBooking(updatedBooking, updatedBooking.Id, "bookings");
                operationResult =
                    OperationResult<string>.Success(callback.returnUrl, StatusCode.Ok, MessageConstant.SuccessMessage.CreatePaymentLinkSuccess);

            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return operationResult;
        }
    }
}