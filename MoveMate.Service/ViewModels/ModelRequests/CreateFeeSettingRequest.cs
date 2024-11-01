using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateFeeSettingRequest
    {
        public int? ServiceId { get; set; }

        public int? HouseTypeId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? Amount { get; set; }

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;

        public string? Type { get; set; }

        public string? Unit { get; set; }

        public double? RangeMin { get; set; }

        public double? RangeMax { get; set; }

        public string? DiscountRate { get; set; }

        public double? FloorPercentage { get; set; }
    }
}
