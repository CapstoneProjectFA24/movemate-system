using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateVoucherRequest
    {
        [Required]
        public int? PromotionCategoryId { get; set; }
        [Required]
        public double? Price { get; set; }
        [Required]
        public string? Code { get; set; }

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
