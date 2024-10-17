using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreatePaymentBookingRequest
    {
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public int ScheduleDetailId { get; set; }
    }
}