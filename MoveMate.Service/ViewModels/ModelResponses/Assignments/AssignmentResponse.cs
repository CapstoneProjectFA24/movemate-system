using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses.Assignments
{
    [FirestoreData]
    public class AssignmentResponse
    {
        [FirestoreProperty] public int Id { get; set; }
        [FirestoreProperty] public int? UserId { get; set; }
        [FirestoreProperty] public int? BookingId { get; set; }
        [FirestoreProperty] public string? Status { get; set; }
        //public double? Price { get; set; }
        [FirestoreProperty] public string? StaffType { get; set; }
        [FirestoreProperty] public bool? IsResponsible { get; set; }
        public string? FailedReason { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}