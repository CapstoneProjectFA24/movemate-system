using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateTruckImgRequest
    {
        [Required]
        public int? TruckId { get; set; }
        [Required]
        public string? ImageUrl { get; set; }
        [Required]
        public string? ImageCode { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
