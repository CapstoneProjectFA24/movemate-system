using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses;

[FirestoreData]
public class FeeDetailResponse
{
    [FirestoreProperty]
    public string? Name { get; set; }
    [FirestoreProperty]
    public string? Description { get; set; }
    [FirestoreProperty]
    public double? Amount { get; set; }
    [FirestoreProperty]
    public int? Quantity { get; set; }
}