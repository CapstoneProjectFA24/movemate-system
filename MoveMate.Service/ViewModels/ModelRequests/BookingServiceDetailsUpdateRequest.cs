using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class BookingServiceDetailsUpdateRequest
    {

        public string? PickupAddress { get; set; }
        public string? PickupPoint { get; set; }
        public string? TypeBooking { get; set; }
        public string? RoomNumber { get; set; }
        public string? FloorsNumber { get; set; }
        public List<ServiceDetailRequest> ServiceDetails { get; set; } = new List<ServiceDetailRequest>();
    }
}
