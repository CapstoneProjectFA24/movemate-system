using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class BookingDetailsResponse
    {
        public int? UserId { get; set; }

        //public int? BookingId { get; set; }

        public string? Status { get; set; }

        public double? Price { get; set; }

        public string? StaffType { get; set; }
    }
}
