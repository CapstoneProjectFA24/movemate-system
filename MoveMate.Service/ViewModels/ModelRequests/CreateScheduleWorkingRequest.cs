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
        public string? Status { get; set; }
        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public string? Type { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
    }
}
