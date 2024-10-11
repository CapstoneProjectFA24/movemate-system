using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Models
{
    public enum PaymentMethod
    {
        [Description("Momo Payment")]
        Momo,
        [Description("VnPay Payment")]
        VnPay,
        [Description("PayOS Payment")]
        PayOS
    }

    public static class PaymentMethodHelper
    {
        // Helper method to get description
        public static string GetEnumDescription(PaymentMethod method)
        {
            var field = method.GetType().GetField(method.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute?.Description ?? method.ToString();
        }
    }
}
