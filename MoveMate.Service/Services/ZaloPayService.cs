using Microsoft.Extensions.Configuration;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly IConfiguration _config;

        public ZaloPayService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateOrderAsync(string userId, int bookingId)
        {
            var appId = _config["ZaloPay:AppId"];
            var key1 = _config["ZaloPay:Key1"];
            var endpoint = _config["ZaloPay:Endpoint"];

            // Tạo các tham số thanh toán và mã hóa dữ liệu
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var orderInfo = new
            {
                app_id = appId,
                app_trans_id = request.OrderId,
                app_time = timestamp,
                amount = request.Amount,
                app_user = request.UserId,
                description = request.Description,
                bank_code = request.BankCode,
                mac = GenerateMac(appId, request.OrderId, timestamp, request.Amount, key1)
            };

            // Gửi yêu cầu tới ZaloPay
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(orderInfo), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to create ZaloPay order");

                return await response.Content.ReadAsStringAsync();
            }
        }

        private string GenerateMac(string appId, string orderId, long timestamp, int amount, string key1)
        {
            var rawData = $"{appId}|{orderId}|{timestamp}|{amount}|{key1}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key1)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
