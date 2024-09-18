using Microsoft.AspNetCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static MoveMate.Service.ThirdPartyService.PayOs.Tyoe;

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public class PayOS
    {
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;

        public PayOS(string clientId, string apiKey, string checksumKey)
        {
            _clientId = clientId;
            _apiKey = apiKey;
            _checksumKey = checksumKey;
        }

        public async Task<string> CreatePaymentLink(PaymentData paymentData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.payos.com/v1/checkout")
            {
                Content = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await SendRequest(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var checkoutResponseData = JsonConvert.DeserializeObject<CheckoutResponseData>(responseData);
                return checkoutResponseData.CheckoutUrl;
            }
            else
            {
                throw new Exception($"Failed to create payment link: {response.StatusCode}");
            }
        }

        public async Task<PaymentLinkData> GetPaymentLinkInformation(long orderId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.payos.com/v1/checkout/{orderId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await SendRequest(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentLinkData>(responseData);
            }
            else
            {
                throw new Exception($"Failed to get payment link information: {response.StatusCode}");
            }
        }

        public async Task<PaymentLinkData> CancelPaymentLink(int orderId, string reason)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://api.payos.com/v1/checkout/{orderId}/cancel")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { reason }), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await SendRequest(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PaymentLinkData>(responseData);
            }
            else
            {
                throw new Exception($"Failed to cancel payment link: {response.StatusCode}");
            }
        }

        public async Task<string> ConfirmWebhook(string webhookUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, webhookUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await SendRequest(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Failed to confirm webhook: {response.StatusCode}");
            }
        }

        public async Task<WebhookData> VerifyPaymentWebhookData(Webhook webhook)
        {
            // Implement webhook verification logic here
            // For now, just return the webhook data
            return webhook.Data;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            using var client = new HttpClient();
            return await client.SendAsync(request);
        }
    }


}
