using MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;
using MoveMate.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Zalo
{
    public interface IZaloPayService
    {
        public ZaloPayOrderCreate BuildZaloPayOrderCreate(string orderId, long amount, string items, string bankCode,
            string embedData, string callbackUrl);

        public Task<ZaloPayOrderResult?> CreateOrder(ZaloPayOrderCreate zaloPayOrder);
        public Task<OrderQueryResult?> QueryOrder(string appTransId);
        public bool ValidateCallback(CallbackOrder callbackOrder);
        public bool ValidateRedirect(RedirectOrder redirectOrder);
    }
}