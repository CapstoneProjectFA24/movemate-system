using AutoMapper.Configuration.Annotations;
using MoveMate.Domain.Models;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
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
     
        public DateTime? BookingAt { get; set; }
        public UserResponse Users { get; set; }


    }
}
