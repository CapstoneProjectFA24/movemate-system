using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateScheduleWorkingRequest
    {
        public string? Name { get; set; }
        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public string? Type { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
 
    }
}
