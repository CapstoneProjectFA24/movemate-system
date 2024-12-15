using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UserInfoRequest
    {
        public string? Type { get; set; }

        public string? ImageUrl { get; set; }
        
        public IFormFile? Image { get; set; }

        public string? Value { get; set; }
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
    }
}
