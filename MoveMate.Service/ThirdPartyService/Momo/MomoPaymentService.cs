using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public MomoPaymentService(
            IOptions<MomoSettings> momoSettings,
            ILogger<MomoPaymentService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _momoSettings = momoSettings.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? CurrentUserPrincipal => _httpContextAccessor.HttpContext?.User;

        public async Task<string> CreatePaymentAsync(MomoPayment payment)
        {
            var ServerUrl = string.Concat(_httpContextAccessor?.HttpContext?.Request.Scheme, "://", _httpContextAccessor?.HttpContext?.Request.Host.ToUriComponent()) ?? throw new Exception("not have url server");
            var requestType = "captureWallet";
            var request = new MomoPaymentRequest();
            request.OrderInfo = payment.Info ?? DefaultOrderInfo;
            request.PartnerCode = _momoSettings.PartnerCode;
            request.IpnUrl = _momoSettings.IpnUrl;
            request.RedirectUrl = $"{ServerUrl}/{_momoSettings.RedirectUrl}?returnUrl={payment.returnUrl}";
            request.Amount = payment.Amount;
            request.OrderId = payment.PaymentReferenceId;
            request.ReferenceId = $"{payment.PaymentReferenceId}";
            request.RequestId = Guid.NewGuid().ToString();
            request.RequestType = requestType;
            request.ExtraData = "s";
            request.AutoCapture = true;
            request.Lang = "vi";


            var rawSignature = $"accessKey={_momoSettings.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={request.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={requestType}";
            request.Signature = GetSignature(rawSignature, _momoSettings.SecretKey);

            var httpContent = new StringContent(JsonSerializerUtils.Serialize(request), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var momoResponse = await httpClient.PostAsync(_momoSettings.PaymentEndpoint, httpContent);
            var responseContent = momoResponse.Content.ReadAsStringAsync().Result;

            if (momoResponse.IsSuccessStatusCode)
            {
                var momoPaymentResponse = JsonSerializerUtils.Deserialize<MomoPaymentResponse>(responseContent);
                _logger.LogInformation($"[Momo payment] Message: {momoPaymentResponse?.Message}");
                if (momoPaymentResponse != null)
                {
                    return momoPaymentResponse.PayUrl;
                }
            }

            throw new Exception($"[Momo payment] Error: There is some error when create payment with momo. {responseContent}");
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
