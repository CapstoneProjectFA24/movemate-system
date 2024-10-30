using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdatePromotionRequest
    {
        [Required]
        public bool IsPublic { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public double DiscountRate { get; set; }
        [Required]
        public double DiscountMax { get; set; }
        [Required]
        public double RequireMin { get; set; }
        [Required]
        public double DiscountMin { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime StartBookingTime { get; set; }
        [Required]
        public DateTime EndBookingTime { get; set; }
        [Required]
        public bool IsInfinite { get; set; }
        [Required]
        public int? ServiceId { get; set; }
    }
}
