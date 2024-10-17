using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.VNPay.Models
{
    public class VnPayPayment
    {
        public string PaymentReferenceId { get; set; } = default!;

        public long Amount { get; set; }

        public string? Info { get; set; }

        public DateTimeOffset Time { get; set; }

        public string returnUrl { get; set; } = default!;
    }
}