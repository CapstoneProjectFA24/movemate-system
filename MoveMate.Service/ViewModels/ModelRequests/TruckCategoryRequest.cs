using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class TruckCategoryRequest
    {
        [Required]
        public string? CategoryName { get; set; }
        [Required]
        public double? MaxLoad { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ImageUrl { get; set; }
        [Required]
        public string? EstimatedLength { get; set; }
        [Required]
        public string? EstimatedWidth { get; set; }
        [Required]
        public string? EstimatedHeight { get; set; }
        [Required]
        public string? Summarize { get; set; }
        [Required]
        public double? Price { get; set; }
        [Required]
        public int? TotalTrips { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
