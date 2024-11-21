using AutoMapper.Configuration.Annotations;
using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class BookingDetailReport
    {
        public int Id { get; set; }

        public int? BookingId { get; set; }

        public string? Name { get; set; }

        public int? Number { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }

        public string? FailReason { get; set; }

        public DateTime? BookingAt { get; set; }
        [JsonIgnore]
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        [JsonIgnore]
        public virtual MoveMate.Domain.Models.Booking? Booking { get; set; }


    }
}
