using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

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
        [FirestoreProperty] public string EstimatedDistance { get; set; }
        [FirestoreProperty] public double Total { get; set; }
        [FirestoreProperty] public double TotalReal { get; set; }
        [FirestoreProperty] public string EstimatedDeliveryTime { get; set; }
        [FirestoreProperty] public bool IsDeposited { get; set; }   
        [FirestoreProperty] public bool IsReported { get; set; }
        [FirestoreProperty] public string ReportedReason { get; set; }
        [FirestoreProperty] public bool IsDeleted { get; set; }
        [FirestoreProperty] public string? CreatedAt { get; set; }
        [FirestoreProperty] public string? BookingAt { get; set; }
        public string CreatedBy { get; set; }
        [FirestoreProperty] public string? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
        public string Review { get; set; }
        [FirestoreProperty] public string TypeBooking { get; set; }
        [FirestoreProperty] public string RoomNumber { get; set; }
        [FirestoreProperty] public string FloorsNumber { get; set; }
        public bool IsManyItems { get; set; }

        [FirestoreProperty] public bool IsCancel { get; set; }

        [FirestoreProperty] public string CancelReason { get; set; }

        public bool IsPorter { get; set; }
        [FirestoreProperty] public bool IsRoundTrip { get; set; }
        public string Note { get; set; }
        [FirestoreProperty] public double TotalFee { get; set; }
        [FirestoreProperty] public bool? IsReviewOnline { get; set; }

        [FirestoreProperty] public string? OrderStatus { get; set; }
        [FirestoreProperty] public string? ProcessStatus { get; set; }
        [FirestoreProperty] public bool? IsUserConfirm { get; set; }
        [FirestoreProperty] public string? ReviewAt { get; set; }
        [FirestoreProperty] public DateTime? EstimatedEndTime { get; set; }
        [FirestoreProperty] public int? TruckNumber { get; set; }
        //public List<ServiceDetailsResponse> ServiceDetails { get; set; }
        [FirestoreProperty] public List<AssignmentResponse> Assignments { get; set; }
        [FirestoreProperty] public List<BookingTrackerResponse> BookingTrackers { get; set; }

        [FirestoreProperty] public List<BookingDetailsResponse> BookingDetails { get; set; }

        [FirestoreProperty] public List<VoucherResponse> Vouchers { get; set; }
        //public virtual List<ServiceDetailsResponse> ServiceDetails { get; set; } = new List<ServiceDetailsResponse>();

        [FirestoreProperty] public virtual ICollection<FeeDetailResponse> FeeDetails { get; set; } = new List<FeeDetailResponse>();

        //public List<HouseTypeResponse> HouseTypes { get; set; }
    }
}