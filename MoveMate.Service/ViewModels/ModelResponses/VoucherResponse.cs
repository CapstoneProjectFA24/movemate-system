using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class VoucherResponse
    {
        [FirestoreProperty] public int Id { get; set; }

        [FirestoreProperty] public int? UserId { get; set; }

        [FirestoreProperty] public int? PromotionCategoryId { get; set; }

        [FirestoreProperty] public int? BookingId { get; set; }

        [FirestoreProperty] public double? Price { get; set; }

        [FirestoreProperty] public string? Code { get; set; }

        [FirestoreProperty] public bool? IsActived { get; set; }
    }
}
