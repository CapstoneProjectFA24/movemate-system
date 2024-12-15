﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateStaffRequest
    {
        public int? RoleId { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? Gender { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }
        
        public IFormFile? Avatar { get; set; }
        
        public DateTime? Dob { get; set; }
        [JsonIgnore]
        public bool? IsBanned { get; set; } = false;
        [JsonIgnore]
        public bool? IsDeleted { get; set; } = false;
        [JsonIgnore]
        public bool? IsAccepted { get; set; } = false;
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<UserInfoRequest> UserInfo { get; set; } = new List<UserInfoRequest>();

    }
}
