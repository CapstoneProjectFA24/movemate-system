using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using Net.payOS.Types;
using Net.payOS;
using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Enums;
using Microsoft.AspNetCore.Http;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Payment.Momo.Models;
using MoveMate.Service.ThirdPartyService.Payment.Momo;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using MoveMate.Service.Utils;
using MoveMate.Domain.Models;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Firebase;
using static Grpc.Core.Metadata;

namespace MoveMate.Service.ThirdPartyService.Payment.PayOs
{
    public class PayOsService : IPayOsService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PayOsService> _logger;
        private readonly PayOS _payOs;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWalletServices _walletServices;
        private readonly IMessageProducer _producer;
        private readonly IFirebaseServices _firebaseServices;

        public PayOsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PayOsService> logger, PayOS payOS,
            IWalletServices walletServices, IHttpContextAccessor httpContextAccessor, IMessageProducer producer, IFirebaseServices firebaseServices)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _payOs = payOS;
            _walletServices = walletServices;
            _httpContextAccessor = httpContextAccessor;
            _producer = producer;
            _firebaseServices = firebaseServices;
        }

        public async Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId, string returnUrl)
        {
            var operationResult = new OperationResult<string>();


            var serverUrl =
                string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://",
                    _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ??
                throw new Exception(MessageConstant.FailMessage.ServerUrl);


            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return operationResult;
            }

            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                operationResult.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                return operationResult;
            }

            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
                if (booking.Status != BookingEnums.DEPOSITING.ToString() &&
                    booking.Status != BookingEnums.COMPLETED.ToString())
                {
                    operationResult.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingStatus);
                    return operationResult;
                }

            string description = "";
            int amount = 0;
            if (booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                amount = (int)booking.Deposit;
                description = "order-deposit";
            }
            else if (booking.Status == BookingEnums.COMPLETED.ToString())
            {
                amount = (int)booking.TotalReal;
                description = "order-payment";
            }


            long newGuid = Guid.NewGuid().GetHashCode();
            do
            {
                newGuid = Guid.NewGuid().GetHashCode();
            } while (newGuid <= 0);

            try
            {
                var urlReturn =
                    $"{serverUrl}/api/v1/payments/payos/callback?returnUrl={returnUrl}&BookingId={bookingId}&Type=order&BuyerEmail={user.Email}&Amount={amount}&userId={userId}";
                var urlCancel = $"{serverUrl}/api/v1/payments/payos/callback?returnUrl={returnUrl}";
                var paymentData = new PaymentData(
                    orderCode: newGuid,
                    amount: amount,
                    description: description,
                    items: null,
                    cancelUrl: urlCancel,
                    returnUrl: urlReturn,
                    buyerName: user.Name,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                    expiredAt: null
                );

                var paymentResult = await _payOs.createPaymentLink(paymentData);
                var paymentUrl = paymentResult.checkoutUrl;

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


        public async Task<OperationResult<string>> CreateRechargeLinkAsync(int userId, double amount, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

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

            var serverUrl =
                string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://",
                    _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ??
                throw new Exception(MessageConstant.FailMessage.ServerUrl);

            long newGuid = Guid.NewGuid().GetHashCode();
            do
            {
                newGuid = Guid.NewGuid().GetHashCode();
            } while (newGuid <= 0);

            try
            {
                var urlReturn =
                    $"{serverUrl}/api/v1/payments/payos/callback?returnUrl={returnUrl}&BookingId={wallet.Id}&Type=wallet&BuyerEmail={user.Email}&Amount={amount}&userId={userId}";
                var paymentData = new PaymentData(
                    orderCode: newGuid,
                    amount: (int)amount,
                    description: "Recharge into wallet",
                    items: null,
                    cancelUrl: "https://movemate-dashboard.vercel.app/payment-status?isSuccess=false",
                    returnUrl: urlReturn,
                    buyerName: user.Name,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                    expiredAt: null
                );

                var paymentResult = await _payOs.createPaymentLink(paymentData);
                var paymentUrl = paymentResult.checkoutUrl;

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
            PayOsPaymentCallbackCommand command)
        {
            var result = new OperationResult<string>();
            string email = command.BuyerEmail;
            var user = await _unitOfWork.UserRepository.GetUserAsyncByEmail(email);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return result;
            }

            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(user.Id);
            if (wallet == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                return result;
            }

            // Check if this transaction has already been processed
            var existingTransaction =
                await _unitOfWork.TransactionRepository.GetByTransactionCodeAsync(command.OrderCode);
            if (existingTransaction != null)
            {
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TransactionSuccess,
                    MessageConstant.SuccessMessage.AlreadyProcess);
                return result; // Exit without updating the wallet
            }

            try
            {
                // Update wallet balance
                wallet.Balance += (float)command.Amount;

                var updateResult = await _walletServices.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }

                // Record the transaction to prevent reprocessing
                var transaction = new MoveMate.Domain.Models.Transaction
                {
                    WalletId = wallet.Id,
                    Amount = (float)command.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = Domain.Enums.PaymentMethod.RECHARGE.ToString(),
                    TransactionCode = command.OrderCode, // Use the callback's TransactionCode
                    CreatedAt = DateTime.Now,
                    Resource = Resource.PayOS.ToString(),
                    PaymentMethod = Resource.PayOS.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
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
            PayOsPaymentCallbackCommand command)
        {
            var result = new OperationResult<string>();

            // Retrieve user by email from the callback command
            var user = await _unitOfWork.UserRepository.GetUserAsyncByEmail(command.BuyerEmail);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return result;
            }

            int bookingId = command.BookingId;
            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, user.Id);
            if (booking == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBooking);
                return result;
            }

            var existingTransaction =
                await _unitOfWork.TransactionRepository.GetByTransactionCodeAsync(command.OrderCode);
            if (existingTransaction != null)
            {
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TransactionSuccess, MessageConstant.SuccessMessage.AlreadyProcess);
                return result; // Exit without updating the wallet
            }

            try
            {
                var payment = new Domain.Models.Payment
                {
                    BookingId = bookingId,
                    Amount = (double)command.Amount,
                    Success = true,
                    BankCode = Resource.PayOS.ToString()
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                string transType = "";
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    transType = Domain.Enums.PaymentMethod.DEPOSIT.ToString();
                    booking.TotalReal = booking.Total - (float)command.Amount;
                }
                else if (booking.Status == BookingEnums.COMPLETED.ToString())
                {
                    transType = Domain.Enums.PaymentMethod.PAYMENT.ToString();
                }

                var transaction = new MoveMate.Domain.Models.Transaction
                {
                    PaymentId = payment.Id,
                    Amount = (float)command.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = transType,
                    TransactionCode = command.OrderCode.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.PayOS.ToString(),
                    PaymentMethod = Resource.PayOS.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                };

                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                if (booking.IsReviewOnline == false && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.REVIEWING.ToString();
                }
                else if (booking.IsReviewOnline == true && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.COMING.ToString();
                }
                else if (booking.Status == BookingEnums.COMPLETED.ToString())
                {
                    booking.Status = BookingEnums.COMPLETED.ToString();
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                    return result;
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                _firebaseServices.SaveBooking(booking, booking.Id, "bookings");

                result = OperationResult<string>.Success(command.returnUrl, StatusCode.Ok, MessageConstant.SuccessMessage.PaymentHandle);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}