using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleBookingDetailsResponse
    {
        public int Id { get; set; }

        public int? BookingId { get; set; }

        public int? UserId { get; set; }

        public int? ScheduleBookingId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? DurationTime { get; set; }

        public double? Amount { get; set; }

        public string? Type { get; set; }
    }
}
