using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateOrderRequest
    {
        [Required] public string BankCode { get; set; }

        [Required] public long Amount { get; set; }

        [Required] public string OrderId { get; set; }

        public string Description { get; set; }

        [Required] public string RedirectUrl { get; set; }

        [Required] public string CallbackUrl { get; set; }
    }
}