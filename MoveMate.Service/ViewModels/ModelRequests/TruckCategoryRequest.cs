using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class TruckCategoryRequest
    {
        public string? CategoryName { get; set; }

        public double? MaxLoad { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? EstimatedLength { get; set; }

        public string? EstimatedWidth { get; set; }

        public string? EstimatedHeight { get; set; }

        public string? Summarize { get; set; }

        public double? Price { get; set; }

        public int? TotalTrips { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
