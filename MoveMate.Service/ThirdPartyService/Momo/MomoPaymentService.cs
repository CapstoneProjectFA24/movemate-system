﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.ThirdPartyService.Momo.Models;
using MoveMate.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Momo
{
    public class MomoPaymentService : IMomoPaymentService
    {
        private const string DefaultOrderInfo = "Thanh toán với Momo";

        private readonly MomoSettings _momoSettings;
        private readonly ILogger<MomoPaymentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;

        public MomoPaymentService(
            IOptions<MomoSettings> momoSettings,
            ILogger<MomoPaymentService> logger,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _momoSettings = momoSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = (UnitOfWork)unitOfWork;
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

            // Retrieve user
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found");
                operationResult.AddError(StatusCode.NotFound, "User not found");
                return operationResult;
            }

            // Retrieve booking
            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {bookingId} for User ID {userId} not found");
                operationResult.AddError(StatusCode.NotFound, "Booking not found");
                return operationResult;
            }

            // Check booking status
            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
            {
                operationResult.AddError(StatusCode.BadRequest, "Booking status must be either WAITING or COMPLETED");
                return operationResult;
            }

            int amount = (booking.Status == "WAITING") ? (int)booking.Deposit : (int)booking.Total;
            var newGuid = Guid.NewGuid();
            try
            {
                // Create payment request data
                var payment = new MomoPayment
                {
                    Amount = amount,
                    Info = "Booking Payment",
                    PaymentReferenceId = newGuid.ToString(),
                    returnUrl = returnUrl,
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
                OrderInfo = payment.Info ?? DefaultOrderInfo,
                PartnerCode = _momoSettings.PartnerCode,
                IpnUrl = _momoSettings.IpnUrl,
                RedirectUrl = _momoSettings.RedirectUrl,
                Amount = payment.Amount,
                OrderId = payment.PaymentReferenceId,
                ReferenceId = $"{payment.PaymentReferenceId}",
                RequestId = Guid.NewGuid().ToString(),
                RequestType = requestType,
                ExtraData = "s",
                AutoCapture = true,
                Lang = "vi"
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
        private static string GetSignature(string text, string key)
        {
            var encoding = new UTF8Encoding();
            var textBytes = encoding.GetBytes(text);
            var keyBytes = encoding.GetBytes(key);

            using var hash = new HMACSHA256(keyBytes);
            var hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

    }
}
