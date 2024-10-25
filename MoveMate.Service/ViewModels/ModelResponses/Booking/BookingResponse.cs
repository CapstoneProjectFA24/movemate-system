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
        public int HouseTypeId { get; set; }
        [FirestoreProperty] public double Deposit { get; set; }
        [FirestoreProperty] public string Status { get; set; }
        public string PickupAddress { get; set; }
        public string PickupPoint { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPoint { get; set; }

        public bool IsUseBox { get; set; }

        public string BoxType { get; set; }
        public string EstimatedDistance { get; set; }
        [FirestoreProperty] public double Total { get; set; }
        [FirestoreProperty] public double TotalReal { get; set; }
        public string EstimatedDeliveryTime { get; set; }
        [FirestoreProperty] public bool IsDeposited { get; set; }
        public bool IsBonus { get; set; }
        public bool IsReported { get; set; }
        public string ReportedReason { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedAt { get; set; }
        [FirestoreProperty] public string? BookingAt { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
        [FirestoreProperty] public string Review { get; set; }

        public string Bonus { get; set; }
        public string TypeBooking { get; set; }

        public string EstimatedAcreage { get; set; }
        public string RoomNumber { get; set; }
        public string FloorsNumber { get; set; }
        public bool IsManyItems { get; set; }

        public bool IsCancel { get; set; }

        public string CancelReason { get; set; }

        public bool IsPorter { get; set; }
        [FirestoreProperty] public bool IsRoundTrip { get; set; }
        public string Note { get; set; }
        [FirestoreProperty] public double TotalFee { get; set; }

        public string FeeInfo { get; set; }

        public bool? IsReviewOnline { get; set; }
        public string? ReviewAt { get; set; }

        //public List<ServiceDetailsResponse> ServiceDetails { get; set; }
        [FirestoreProperty] public List<AssignmentResponse> Assignments { get; set; }
        public List<BookingTrackerResponse> BookingTrackers { get; set; }

        public List<BookingDetailsResponse> BookingDetails { get; set; }


        //public virtual List<ServiceDetailsResponse> ServiceDetails { get; set; } = new List<ServiceDetailsResponse>();

        public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();

        //public List<HouseTypeResponse> HouseTypes { get; set; }
    }
}