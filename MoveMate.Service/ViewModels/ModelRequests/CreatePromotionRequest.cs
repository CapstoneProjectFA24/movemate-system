using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreatePromotionRequest
    {
        public bool? IsPublic { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        public double? DiscountRate { get; set; }
        [Required]
        public double? DiscountMax { get; set; }
        [Required]
        public double? RequireMin { get; set; }
        [Required]
        public double? DiscountMin { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        public int? Quantity { get; set; }

        public DateTime? StartBookingTime { get; set; }

        public DateTime? EndBookingTime { get; set; }

        public bool? IsInfinite { get; set; }
        [Required]
        public int ServiceId { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;

        public virtual ICollection<VoucherRequest> Vouchers { get; set; } = new List<VoucherRequest>();
    }
}
