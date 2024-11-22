using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateGroupRequest
    {
        public string? Name { get; set; }
        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public int? DurationTimeActived { get; set; }
    }
}
