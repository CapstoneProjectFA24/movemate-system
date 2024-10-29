using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class VoucherRequest
    {
        public int? PromotionCategoryId { get; set; }

        public double? Price { get; set; }

        public string? Code { get; set; }

        public bool? IsActived { get; set; }
    }
}
