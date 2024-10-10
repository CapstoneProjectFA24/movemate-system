using Microsoft.Extensions.Configuration;
using MoveMate.Service.ThirdPartyService.Zalo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Zalo
{
    public class ZaloPaySDK
    {
        private string callback_url = "http://localhost:5210/callback";
        private string redirect_url = "http://localhost:5210/redirect";
        private readonly IZaloPayService _zaloPayServices;
        private readonly IConfiguration _configuration;

        public ZaloPaySDK(IZaloPayService zaloPayServices, IConfiguration configuration)
        {
            _zaloPayServices = zaloPayServices;
            _configuration = configuration;
        }

        public async Task<string?> GeneratePaymentLink(string orderId, long amount, string bankCode)
        {
            var zaloPayOrderCreate = _zaloPayServices.BuildZaloPayOrderCreate(orderId, amount, "[]", bankCode, "{}", _configuration["ZaloPay:CallbackUrl"]);
            var result = await _zaloPayServices.CreateOrder(zaloPayOrderCreate);

            return result?.OrderUrl;
        }

        public async Task<ZaloPayOrderResult?> CreateOrder(string bankCode)
        {
            var rnd = new Random();
            var embedData = new
            {
                redirecturl = redirect_url
            };
            var items = new[] { new { } };
            var orderId = rnd.Next(1000000);
            var zaloPayOrderCreate = _zaloPayServices.BuildZaloPayOrderCreate("123456", 50000, "[]", "SACOMBANK", "{}", "http://localhost/callback");
            var result = await _zaloPayServices.CreateOrder(zaloPayOrderCreate);
            return result;
        }

        public async Task<OrderQueryResult?> QueryOrder(string appTransId)
        {
            OrderQueryResult? result = await _zaloPayServices.QueryOrder(appTransId);

            return result;
        }

        public bool ChecksumData(RedirectOrder redirectOrder)
        {
            return _zaloPayServices.ValidateRedirect(redirectOrder);
        }

        public bool CheckMac(CallbackOrder callbackOrder)
        {
            return _zaloPayServices.ValidateCallback(callbackOrder);
        }
    }
}
