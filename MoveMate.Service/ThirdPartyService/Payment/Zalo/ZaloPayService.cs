using Microsoft.Extensions.Configuration;
using MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;
using MoveMate.Service.ThirdPartyService.Payment.Zalo.ZaloPayHelper.Crypto;
using MoveMate.Service.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Zalo
{
    public class ZaloPayServices : IZaloPayService
    {
        private const string QueryOrderUrl = "/query";
        private readonly string _appUser = "PruGift";
        private readonly string _key2;
        private const string BaseUrl = "https://sb-openapi.zalopay.vn/v2";
        private const string CreateOrderUrl = "/create";
        private readonly HttpClient _httpClient;
        private readonly int _appId;
        private readonly string _key1;

        public ZaloPayServices(IConfiguration config)
        {
            _appId = int.Parse(config["ZaloPay:AppId"]);
            _key1 = config["ZaloPay:Key1"];
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/json; charset=UTF-8");
        }

        public ZaloPayOrderCreate BuildZaloPayOrderCreate(string orderId, long amount, string items, string bankCode,
            string embedData, string callbackUrl)
        {
            var appTransId = DateTime.Now.ToString("yyMMdd") + "_" + orderId;
            var data = $"{_appId}|{appTransId}|{amount}|{embedData}|{items}";
            var mac = HmacHelper.Compute(_key1, data);

            return new ZaloPayOrderCreate
            {
                AppId = _appId,
                AppTransId = appTransId,
                Amount = amount,
                Item = items,
                Description = $"Order #{orderId}",
                EmbedData = embedData,
                BankCode = bankCode,
                Mac = mac,
                CallbackUrl = callbackUrl
            };
        }

        public async Task<ZaloPayOrderResult?> CreateOrder(ZaloPayOrderCreate orderCreate)
        {
            var content = new StringContent(JsonConvert.SerializeObject(orderCreate), Encoding.UTF8,
                "application/json");
            var response = await _httpClient.PostAsync(CreateOrderUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ZaloPayOrderResult>(responseString);
        }

        public async Task<OrderQueryResult?> QueryOrder(string appTransId)
        {
            var data = $"{_appId}|{appTransId}|{_key1}";
            var mac = HmacHelper.Compute(_key1, data);
            var orderQuery = new OrderQuery
            {
                AppId = _appId,
                AppTransId = appTransId,
                Mac = mac
            };
            return await PostMethod<OrderQueryResult>(QueryOrderUrl, orderQuery);
        }

        private async Task<T?> PostMethod<T>(string url, IBaseFormRequest form)
        {
            var content = new FormUrlEncodedContent(form.ToDictionary());
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvertHelper.DeserializeObject<T>(responseString);
        }

        public bool ValidateCallback(CallbackOrder callbackOrder)
        {
            var mac = HmacHelper.Compute(_key2, callbackOrder.Data);
            return mac.Equals(callbackOrder.Mac);
        }

        public bool ValidateRedirect(RedirectOrder redirectOrder)
        {
            var checksumData =
                $"{redirectOrder.Appid}|{redirectOrder.Apptransid}|{redirectOrder.Pmcid}|{redirectOrder.Bankcode}|{redirectOrder.Amount}|{redirectOrder.Discountamount}|{redirectOrder.Status}";
            var mac = HmacHelper.Compute(_key2, checksumData);
            return mac.Equals(redirectOrder.Checksum);
        }
    }
}