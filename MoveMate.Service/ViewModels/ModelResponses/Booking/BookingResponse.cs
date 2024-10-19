using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    [FirestoreData]
    public class BookingResponse
    {
        [FirestoreProperty] public int Id { get; set; }

        [FirestoreProperty] public int UserId { get; set; }
        [FirestoreProperty] public int HouseTypeId { get; set; }
        [FirestoreProperty] public double Deposit { get; set; }
        [FirestoreProperty] public string Status { get; set; }
        [FirestoreProperty] public string PickupAddress { get; set; }
        [FirestoreProperty] public string PickupPoint { get; set; }
        [FirestoreProperty] public string DeliveryAddress { get; set; }
        [FirestoreProperty] public string DeliveryPoint { get; set; }

        public bool IsUseBox { get; set; }

        public string BoxType { get; set; }
        [FirestoreProperty] public string EstimatedDistance { get; set; }
        [FirestoreProperty] public double Total { get; set; }
        [FirestoreProperty] public double TotalReal { get; set; }
        [FirestoreProperty] public string EstimatedDeliveryTime { get; set; }
        [FirestoreProperty] public bool IsDeposited { get; set; }
        [FirestoreProperty] public bool IsBonus { get; set; }
        [FirestoreProperty] public bool IsReported { get; set; }
        [FirestoreProperty] public string ReportedReason { get; set; }
        [FirestoreProperty] public bool IsDeleted { get; set; }
        [FirestoreProperty] public string? CreatedAt { get; set; }
        [FirestoreProperty] public string? BookingAt { get; set; }
        public string CreatedBy { get; set; }
        [FirestoreProperty] public string? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
        [FirestoreProperty] public string Review { get; set; }

        public string Bonus { get; set; }
        [FirestoreProperty] public string TypeBooking { get; set; }

        public string EstimatedAcreage { get; set; }
        [FirestoreProperty] public string RoomNumber { get; set; }
        [FirestoreProperty] public string FloorsNumber { get; set; }
        [FirestoreProperty] public bool IsManyItems { get; set; }

        public bool IsCancel { get; set; }

        public string CancelReason { get; set; }

        public bool IsPorter { get; set; }
        [FirestoreProperty] public bool IsRoundTrip { get; set; }
        [FirestoreProperty] public string Note { get; set; }
        [FirestoreProperty] public double TotalFee { get; set; }

        public string FeeInfo { get; set; }
        [FirestoreProperty]
        public bool? IsReviewOnline { get; set; }
        //public DateTime ReviewAt { get; set; }

        //public List<ServiceDetailsResponse> ServiceDetails { get; set; }
        [FirestoreProperty] public List<BookingDetailsResponse> BookingDetails { get; set; }
        [FirestoreProperty] public List<BookingTrackerResponse> BookingTrackers { get; set; }

        [FirestoreProperty]
        public virtual List<ServiceDetailsResponse> ServiceDetails { get; set; } = new List<ServiceDetailsResponse>();

        [FirestoreProperty]
        public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();

        //public List<HouseTypeResponse> HouseTypes { get; set; }
    }
}