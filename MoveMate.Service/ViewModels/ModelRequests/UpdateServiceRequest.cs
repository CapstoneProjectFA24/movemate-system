using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateServiceRequest
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActived { get; set; }

        public string? ImageUrl { get; set; }

        public double? DiscountRate { get; set; }

        public double? Amount { get; set; }

        public string? Type { get; set; }

        public bool? IsQuantity { get; set; }

        public int? QuantityMax { get; set; }

        public int? TruckCategoryId { get; set; }
        public int? ParentServiceId { get; set; }
    }
}
