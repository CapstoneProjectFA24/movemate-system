using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ChangeBookingAtRequest
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Filed is required")]
        public DateTime? BookingAt { get; set; }

        public bool IsBookingAtValid()
        {
            return BookingAt.HasValue && BookingAt.Value >= DateTime.Now;
        }
    }
}
