using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateScheduleBookingRequest
    {

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;

        public string? Shard { get; set; }
 
    }
}
