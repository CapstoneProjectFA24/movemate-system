using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ServiceDetailsResponse
    {
        public int? ServiceId { get; set; }

        public int? BookingId { get; set; }

        public int? Quantity { get; set; }

        public double? Price { get; set; }

        public bool? IsQuantity { get; set; }
        
        public string? Description { get; set; }

    }
}
