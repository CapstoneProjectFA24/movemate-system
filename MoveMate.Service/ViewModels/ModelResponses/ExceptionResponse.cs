using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ExceptionResponse : BookingTrackerResponse
    {
        public double? Deposit { get; set; }
        public string? BookingStatus { get; set; }
        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public double? Total { get; set; }
        public double? TotalReal { get; set; }
        public string? Note { get; set; }
        public DateTime? BookingAt { get; set; }
        public bool? IsReviewOnline { get; set; } = false;
        public bool? BookingIsInsurance { get; set; } = false;
        public UserExceptionResponse Owner { get; set; }
        public virtual ICollection<AssignmentResponse> Assignments { get; set; }
    }
}
