using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class VoucherRequest
    {
        public double? Price { get; set; }

        public string? Code { get; set; }

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
    }
}
