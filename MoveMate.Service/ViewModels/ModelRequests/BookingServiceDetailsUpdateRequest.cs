using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class BookingServiceDetailsUpdateRequest
    {

        public int? TruckCategoryId { get; set; }
        public int? HouseTypeId { get; set; }
        public string? PickupAddress { get; set; }
        public string? PickupPoint { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPoint { get; set; }
        public string? EstimatedDistance { get; set; }
        public double? EstimatedDeliveryTime { get; set; }
        public bool? IsRoundTrip { get; set; }
        public string? TypeBooking { get; set; }
        public string? RoomNumber { get; set; }

        public string? FloorsNumber { get; set; }
        public string? Note { get; set; }
        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? BookingAt { get; set; } 
        public List<BookingDetailRequest> BookingDetails { get; set; } = new List<BookingDetailRequest>();
    }
}
