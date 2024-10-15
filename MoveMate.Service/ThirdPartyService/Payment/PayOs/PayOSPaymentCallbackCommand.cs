using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.PayOs
{
    public class PayOsPaymentCallbackCommand
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public int BookingId { get; set; }
        public bool Cancel { get; set; }
        public int ResultCode { get; set; } = default!;
        public string Status { get; set; }
        public string OrderCode { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public string BuyerEmail { get; set; }
        public bool IsSuccess => ResultCode == 0;
        public string returnUrl { get; set; }
    }

}
