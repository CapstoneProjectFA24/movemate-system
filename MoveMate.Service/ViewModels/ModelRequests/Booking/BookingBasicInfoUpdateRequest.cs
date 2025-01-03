﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests.Booking
{
    public class BookingBasicInfoUpdateRequest
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public double? Deposit { get; set; }
        public string? Status { get; set; }
        public string? PickupAddress { get; set; }
        public string? PickupPoint { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryPoint { get; set; }
        public string? EstimatedDistance { get; set; }
        public double? Total { get; set; }
        public double? TotalReal { get; set; }
        public string? EstimatedDeliveryTime { get; set; }
        public bool? IsDeposited { get; set; }
        public bool? IsBonus { get; set; }
        public bool? IsReported { get; set; }
        public string? ReportedReason { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Review { get; set; }
        //public DateTime? ReviewAt { get; set; }
        public string? Bonus { get; set; }
        public string? TypeBooking { get; set; }
        public string? RoomNumber { get; set; }
        public string? FloorsNumber { get; set; }
        public bool? IsManyItems { get; set; }
        public bool? IsCancel { get; set; }
        public string? CancelReason { get; set; }
        public bool? IsPorter { get; set; }
        public bool? IsRoundTrip { get; set; }
        public string? Note { get; set; }
        public double? TotalFee { get; set; }
        public string? FeeInfo { get; set; }
        public int? HouseTypeId { get; set; }
        public DateTime? BookingAt { get; set; }
        public bool? IsReviewOnline { get; set; }
        public bool? IsUserConfirm { get; set; }

    }
}
