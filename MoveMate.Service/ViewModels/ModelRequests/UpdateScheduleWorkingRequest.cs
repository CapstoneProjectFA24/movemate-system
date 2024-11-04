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
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;


        public string? Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
