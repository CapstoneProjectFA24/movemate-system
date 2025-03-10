﻿using System;
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
        public string? EstimatedLenght { get; set; }
        [Required]
        public string? EstimatedWidth { get; set; }
        [Required]
        public string? EstimatedHeight { get; set; }

        public string? Summarize { get; set; }

        public double? Price { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
