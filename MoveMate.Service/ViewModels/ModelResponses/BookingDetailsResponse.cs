using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class BookingDetailsResponse
    {
        [FirestoreProperty] public int Id { get; set; }
        [FirestoreProperty] public int? UserId { get; set; }
        [FirestoreProperty] public int? BookingId { get; set; }
        [FirestoreProperty] public string? Status { get; set; }
        [FirestoreProperty] public double? Price { get; set; }
        [FirestoreProperty] public string? StaffType { get; set; }
        [FirestoreProperty] public bool? IsResponsible { get; set; }
    }
}