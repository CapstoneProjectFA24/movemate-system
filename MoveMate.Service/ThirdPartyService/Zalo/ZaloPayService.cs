using Microsoft.Extensions.Configuration;
using MoveMate.Service.ThirdPartyService.Zalo.Models;
using MoveMate.Service.ThirdPartyService.Zalo.ZaloPayHelper.Crypto;
using MoveMate.Service.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Zalo
{
    public class ZaloPayServices : IZaloPayService
    {
        private const string BaseUrl = "https://sb-openapi.zalopay.vn/v2";
        private const string CreateOrderUrl = "/create";
        private const string QueryOrderUrl = "/query";
        private readonly HttpClient _httpClient;
        private readonly string _appUser = "PruGift";
        private readonly int _appId;
        private readonly string _key1;
        private readonly string _key2;

        public ZaloPayServices(IConfiguration config)
        {
            _appId = int.Parse(config.GetSection("ZaloPay:AppId").Value ?? "0");
            _key1 = config.GetSection("ZaloPay:Key1").Value ?? "";
            _key2 = config.GetSection("ZaloPay:Key2").Value ?? "";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=UTF-8");
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public ZaloPayOrderCreate BuildZaloPayOrderCreate(string orderId, long amount, string items, string bankCode,
            string embedData, string callbackUrl)
        {
            DateTimeOffset now = DateTime.UtcNow;
            long appTime = now.ToUnixTimeMilliseconds();
            string appTransId = DateTime.Now.ToString("yyMMdd") + "_" + orderId;
            string data = $"{_appId}|{appTransId}|{_appUser}|{amount}|{appTime}|{embedData}|{items}";
            ZaloPayOrderCreate orderCreate = new ZaloPayOrderCreate
            {
                AppId = _appId,
                AppUser = _appUser,
                AppTransId = appTransId,
                AppTime = appTime,
                Amount = amount,
                Item = items,
                Description = "Thanh toán đơn hàng #" + orderId,
                EmbedData = embedData,
                BankCode = bankCode,
                Mac = HmacHelper.Compute(_key1, data),
                CallbackUrl = callbackUrl
            };
            return orderCreate;
        }

        public async Task<ZaloPayOrderResult?> CreateOrder(ZaloPayOrderCreate zaloPayOrder)
        {
            return await PostMethod<ZaloPayOrderResult>(CreateOrderUrl, zaloPayOrder);
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
            var checksumData = $"{redirectOrder.Appid}|{redirectOrder.Apptransid}|{redirectOrder.Pmcid}|{redirectOrder.Bankcode}|{redirectOrder.Amount}|{redirectOrder.Discountamount}|{redirectOrder.Status}";
            var mac = HmacHelper.Compute(_key2, checksumData);
            return mac.Equals(redirectOrder.Checksum);
        }
    }
}
