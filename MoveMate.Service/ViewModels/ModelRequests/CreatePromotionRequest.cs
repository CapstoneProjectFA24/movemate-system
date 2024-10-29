using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreatePromotionRequest
    {
        public bool? IsPublic { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public double? DiscountRate { get; set; }

        public double? DiscountMax { get; set; }

        public double? RequireMin { get; set; }

        public double? DiscountMin { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public int? Quantity { get; set; }

        public DateTime? StartBookingTime { get; set; }

        public DateTime? EndBookingTime { get; set; }

        public bool? IsInfinite { get; set; }

        public int? ServiceId { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;

        public virtual ICollection<VoucherRequest> Vouchers { get; set; } = new List<VoucherRequest>();
    }
}
