using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public class Tyoe
    {
        public class CheckoutResponseData
        {
            public string OrderId { get; set; }
            public string PaymentLink { get; set; }
            public string CheckoutUrl { get; set; }
            // Add other properties as needed
        }

        public class PaymentLinkData
        {
            public string OrderId { get; set; }
            public string PaymentLink { get; set; }
            // Add other properties as needed
        }

        public class WebhookData
        {
            public string Event { get; set; }
            public string Data { get; set; }
            // Add other properties as needed
        }

        public class Webhook
        {
            public string Url { get; set; }
            public WebhookData Data { get; set; }
            // Add other properties as needed
        }
    }
}
