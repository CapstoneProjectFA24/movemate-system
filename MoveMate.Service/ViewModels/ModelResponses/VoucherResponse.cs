using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class VoucherResponse
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? PromotionCategoryId { get; set; }

        public int? BookingId { get; set; }

        public double? Price { get; set; }

        public string? Code { get; set; }

        public bool? IsActived { get; set; }
    }
}
