using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateTruckImgRequest
    {
        public int? TruckId { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImageCode { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
