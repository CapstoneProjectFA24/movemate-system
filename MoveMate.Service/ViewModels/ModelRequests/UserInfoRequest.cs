using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UserInfoRequest
    {
        public Byte? Type { get; set; }

        public string? ImageUrl { get; set; }

        public string? Value { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
