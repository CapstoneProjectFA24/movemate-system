using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.PayOs
{
    public class PayOSPaymentCallbackCommand
    {

        public string returnUrl { get; set; } = default!;

        public bool IsSuccess { get; set; }

    }
}
