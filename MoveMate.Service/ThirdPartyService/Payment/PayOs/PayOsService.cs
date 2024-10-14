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
       
        public PayOsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PayOsService> logger, PayOS payOS, IWalletServices walletServices, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _payOs = payOS;
            _walletServices = walletServices;
            _httpContextAccessor = httpContextAccessor;
           
        }

        public async Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId, string returnUrl)
        {
            var operationResult = new OperationResult<string>();



            var serverUrl = string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://", _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ?? throw new Exception("Server URL is not available");


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

            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
                if (booking.Status != BookingEnums.DEPOSITING.ToString() && booking.Status != BookingEnums.COMPLETED.ToString())
                {
                    operationResult.AddError(StatusCode.BadRequest, "Booking status must be either DEPOSITING or COMPLETED");
                    return operationResult;
                }
            string description = "";
            int amount = 0;
            if (booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                amount = (int)booking.Deposit;
                description = "order-deposit";
            }
            else if(booking.Status == BookingEnums.COMPLETED.ToString())
            {
                amount = (int)booking.TotalReal;
                description = "order-payment";
            }

            
            long newGuid = Guid.NewGuid().GetHashCode(); // Use your method for generating newGuid

            // Generate a unique order code
            long orderCode = PaymentUtils.GenerateOrderCode(bookingId, newGuid);
            try
            {
                
                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: amount,
                    description: description,
                    items: null,
                    cancelUrl: "https://movemate-dashboard.vercel.app/payment-status?isSuccess=false",
                    returnUrl: serverUrl+ "/api/v1/payments/payos/callback",
                    buyerName: user.Name,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                expiredAt: null
                );

                var paymentResult = await _payOs.createPaymentLink(paymentData);
                var paymentUrl = paymentResult.checkoutUrl;

                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }


        public async Task<OperationResult<string>> CreateRechargeLinkAsync(int userId, double amount, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            if (amount <= 0)
            {
                operationResult.AddError(StatusCode.BadRequest, "Amount must be greater than zero");
                return operationResult;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                operationResult.AddError(StatusCode.NotFound, "User not found");
                return operationResult;
            }
            var serverUrl = string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://", _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ?? throw new Exception("Server URL is not available");

            long newGuid = Guid.NewGuid().GetHashCode();
            do
            {
                newGuid = Guid.NewGuid().GetHashCode();
            } while (newGuid <= 0);
            try
            {
                var urlReturn = $"{serverUrl}/api/v1/payments/payos/callback?returnUrl={returnUrl}&Type=wallet&BuyerEmail={user.Email}&Amount={amount}";
                var paymentData = new PaymentData(
                    orderCode: newGuid,
                    amount: (int)amount,
                    description: "Wallet",
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

                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }

        public async Task<OperationResult<string>> HandleWalletPaymentAsync(HttpContext context, PayOsPaymentCallbackCommand command)
        {
            var result = new OperationResult<string>();
            string email = command.BuyerEmail;
            var user = await _unitOfWork.UserRepository.GetUserAsyncByEmail(email);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, "User not found");
                return result;
            }

            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(user.Id);
            if (wallet == null)
            {
                result.AddError(StatusCode.NotFound, "Wallet not found");
                return result;
            }

            // Check if this transaction has already been processed
            var existingTransaction = await _unitOfWork.TransactionRepository.GetByTransactionCodeAsync(command.OrderCode);
            if (existingTransaction != null)
            {
                result.AddResponseStatusCode(StatusCode.Ok, "Transaction has already been processed.", "Already Processed");
                return result; // Exit without updating the wallet
            }

            try
            {
                // Update wallet balance
                wallet.Balance += (float)command.Amount;

                var updateResult = await _walletServices.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, "Failed to update wallet balance");
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

                result.AddResponseStatusCode(StatusCode.Ok, "Wallet updated successfully", "Success");
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, "An internal server error occurred: " + ex.Message);
            }

            return result;
        }

    }
}
