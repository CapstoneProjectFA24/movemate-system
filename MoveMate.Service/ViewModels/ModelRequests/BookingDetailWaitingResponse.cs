using Google.Cloud.Firestore;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class BookingDetailWaitingResponse
    {

        public int Id { get; set; }
        public int? ServiceId { get; set; }
        public int? BookingId { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<AssignmentResponse> Assignments { get; set; }
    }
}
