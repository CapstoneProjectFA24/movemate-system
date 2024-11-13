using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateScheduleWorkingRequest
    {

        public string? Status { get; set; }

        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; } 


        public string? Type { get; set; }

        public TimeOnly? StartDate { get; set; }

        public TimeOnly? EndDate { get; set; }
    }
}
