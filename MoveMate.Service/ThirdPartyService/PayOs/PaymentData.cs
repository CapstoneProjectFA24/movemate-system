using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public class PaymentData
    {
        public string OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public ItemData Item { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
