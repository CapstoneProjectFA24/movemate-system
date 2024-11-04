using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleBookingResponse
    {
        public int Id { get; set; }

        public bool? IsActived { get; set; }

        public string? Shard { get; set; }

        public virtual ICollection<ScheduleBookingDetailsResponse> ScheduleBookingDetails { get; set; } = new List<ScheduleBookingDetailsResponse>();
    }
}
