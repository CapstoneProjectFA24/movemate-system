using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class StatusRequest
    {
        [Required] 
        public string Status { get; set; }

        public List<AddVoucherRequest> Vouchers { get; set; } = new List<AddVoucherRequest>();

        public bool IsValid()
        {
            return AreVouchersUnique();
        }

        public bool AreVouchersUnique()
        {

            var promotionIds = Vouchers.Select(v => v.PromotionCategoryId).ToList();
            return promotionIds.Distinct().Count() == promotionIds.Count;
        }
    }
}
