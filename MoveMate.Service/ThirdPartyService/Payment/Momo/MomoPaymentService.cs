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
using MoveMate.Service.ThirdPartyService.Payment.Momo.Models;
using MoveMate.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Momo
{
    public class MomoPaymentService : IMomoPaymentService
    {
        private const string DefaultOrderInfo = "Thanh toán với Momo";

        private readonly MomoSettings _momoSettings;
        private readonly ILogger<MomoPaymentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IWalletServices _walletService;

        public MomoPaymentService(
            IOptions<MomoSettings> momoSettings,
            ILogger<MomoPaymentService> logger,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IWalletServices walletServices)
        {
            _momoSettings = momoSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _walletService = walletServices;
        }

        public ClaimsPrincipal? CurrentUserPrincipal => _httpContextAccessor.HttpContext?.User;

        public async Task<OperationResult<string>> CreatePaymentWithMomoAsync(int bookingId, int userId, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            // Validate parameters
            if (string.IsNullOrEmpty(returnUrl))
            {
                operationResult.AddError(StatusCode.BadRequest, "Return URL is required");
                return operationResult;
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                operationResult.AddError(StatusCode.NotFound, "User not found");
                return operationResult;
            }
            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {bookingId} for User ID {userId} not found");
                operationResult.AddError(StatusCode.NotFound, "Booking not found");
                return operationResult;
            }
            if (booking.Status != BookingEnums.DEPOSITING.ToString() && booking.Status != BookingEnums.COMPLETED.ToString())
            {
                operationResult.AddError(StatusCode.BadRequest, "Booking status must be either DEPOSITING or COMPLETED");
                return operationResult;
            }

            int amount=  0; 
            if (booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                amount = (int)booking.Deposit;
            }
            else if (booking.Status == BookingEnums.COMPLETED.ToString()) 
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
                var paymentUrl = await CreatePaymentAsync(payment);

                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An internal server error occurred while creating the payment");
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }

        public async Task<string> CreatePaymentAsync(MomoPayment payment)
        {
            var serverUrl = string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://", _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ?? throw new Exception("Server URL is not available");
            var requestType = "payWithATM";
            var request = new MomoPaymentRequest
            {
                OrderInfo = payment.Info,
                PartnerCode = _momoSettings.PartnerCode,
                IpnUrl = _momoSettings.IpnUrl,
                RedirectUrl = $"{serverUrl}/{_momoSettings.RedirectUrl}?returnUrl={payment.returnUrl}",
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

            var rawSignature = $"accessKey={_momoSettings.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={request.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={requestType}";
            request.Signature = GetSignature(rawSignature, _momoSettings.SecretKey);

            var httpContent = new StringContent(JsonSerializerUtils.Serialize(request), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var momoResponse = await httpClient.PostAsync(_momoSettings.PaymentEndpoint, httpContent);
            var responseContent = await momoResponse.Content.ReadAsStringAsync();

            if (momoResponse.IsSuccessStatusCode)
            {
                var momoPaymentResponse = JsonSerializerUtils.Deserialize<MomoPaymentResponse>(responseContent);
                _logger.LogInformation($"[Momo payment] Message: {momoPaymentResponse?.Message}");
                if (momoPaymentResponse != null)
                {
                    return momoPaymentResponse.PayUrl;
                }
            }

            _logger.LogError($"[Momo payment] Error: There was an error creating payment with Momo. Response: {responseContent}");
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

            var newGuid = Guid.NewGuid();
            try
            {
                // Create payment request data for Momo
                var payment = new MomoPayment
                {
                    Amount = (int)amount,
                    Info = "wallet",
                    PaymentReferenceId = $"wallet-{userId}-{newGuid}",
                    returnUrl =returnUrl,
                    ExtraData = userId.ToString()
                };

                // Generate Momo payment link
                var paymentUrl = await CreatePaymentAsync(payment);

                // Add logic to update user's wallet if payment is successful
                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An internal server error occurred while adding funds to wallet");
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }






        public async Task<OperationResult<string>> HandleWalletPaymentAsync(HttpContext context, MomoPaymentCallbackCommand command)
        {
            var result = new OperationResult<string>();
            int userId = int.Parse(command.ExtraData);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, "User not found");
                return result;
            }

            // Tìm wallet của user
            var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
            if (wallet == null)
            {
                result.AddError(StatusCode.NotFound, "Wallet not found");
                return result;
            }

            try
            {
                // Cập nhật số tiền vào ví từ MomoPaymentCallbackCommand.Amount
                wallet.Balance += (float)command.Amount;

                var updateResult = await _walletService.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);

                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, "Failed to update wallet balance");
                    return result;
                }

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
