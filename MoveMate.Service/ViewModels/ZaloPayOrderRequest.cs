using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels
{
    public class ZaloPayOrderRequest
    {
        public string OrderId { get; set; }
        public int Amount { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string BankCode { get; set; }
    }
}