using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses;

[FirestoreData]
public class FeeDetailResponse
{
    [FirestoreProperty] public int Id { get; set; }
    [FirestoreProperty] public int? BookingId { get; set; }
    [FirestoreProperty] public int? FeeSettingId { get; set; }
    [FirestoreProperty] public string? Name { get; set; }
    [FirestoreProperty] public string? Description { get; set; }
    [FirestoreProperty] public double? Amount { get; set; }
    [FirestoreProperty] public int? Quantity { get; set; }
}