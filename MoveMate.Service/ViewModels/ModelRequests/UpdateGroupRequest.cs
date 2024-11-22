using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateGroupRequest
    {
        public string? Name { get; set; }
        
        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public int? DurationTimeActived { get; set; }
    }
}
