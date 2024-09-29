using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreatePaymentRequest
    {
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }
        public string ExtraData { get; set; } = ""; // Optional
    }
}
