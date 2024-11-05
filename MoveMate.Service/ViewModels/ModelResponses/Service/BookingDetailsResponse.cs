using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class BookingDetailsResponse
    {
        [FirestoreProperty] public int Id { get; set; }
        [FirestoreProperty] public int? ServiceId { get; set; }
        [FirestoreProperty] public int? BookingId { get; set; }
        [FirestoreProperty] public int? Quantity { get; set; }
        [FirestoreProperty] public double? Price { get; set; }
        [FirestoreProperty] public string? Status { get; set; }
        [FirestoreProperty] public string? Type { get; set; }
        [FirestoreProperty] public string? Name { get; set; }
        [FirestoreProperty] public string? Description { get; set; }
        [FirestoreProperty] public string? ImageUrl { get; set; }
    }
}