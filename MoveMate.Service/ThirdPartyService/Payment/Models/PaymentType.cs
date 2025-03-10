﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Models
{
    public enum PaymentType
    {
        [Description("Momo Payment")] Momo,
        [Description("VnPay Payment")] VnPay,
        [Description("PayOS Payment")] PayOS,
        [Description("Wallet Payment")] Wallet

    }

    public static class PaymentMethodHelper
    {
        // Helper method to get description
        public static string GetEnumDescription(PaymentType method)
        {
            var field = method.GetType().GetField(method.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute?.Description ?? method.ToString();
        }
    }
}