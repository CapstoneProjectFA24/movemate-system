using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Momo.Models
{
    public class MomoPayment
    {
        public string PaymentReferenceId { get; set; } = default!;

        public long Amount { get; set; }

        public string? Info { get; set; }

        public string returnUrl { get; set; } = default!;
    }
}
