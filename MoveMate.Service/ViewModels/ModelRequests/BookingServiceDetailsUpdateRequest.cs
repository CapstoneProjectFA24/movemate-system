using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class BookingServiceDetailsUpdateRequest
    {
        public int BookingId { get; set; }
        public List<ServiceDetailRequest> ServiceDetails { get; set; } = new List<ServiceDetailRequest>();
    }
}
