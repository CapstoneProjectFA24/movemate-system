﻿using System;
using System.Collections.Generic;
using MoveMate.Domain.Utils;

namespace MoveMate.Domain.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? HouseTypeId { get; set; }

    public double? Deposit { get; set; }

    public string? Status { get; set; }

    public string? PickupAddress { get; set; }

    public string? PickupPoint { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? DeliveryPoint { get; set; }

    public string? EstimatedDistance { get; set; }

    public double? Total { get; set; }

    public double? TotalReal { get; set; }

    public double? EstimatedDeliveryTime { get; set; }

    public bool? IsDeposited { get; set; } = false;

    public bool? IsReported { get; set; } =  false;

    public string? ReportedReason { get; set; }

    public bool? IsDeleted { get; set; } = false;

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Review { get; set; }

    public string? TypeBooking { get; set; }

    public string? RoomNumber { get; set; }

    public string? FloorsNumber { get; set; }

    public string? IsManyItems { get; set; }

    public bool? IsCancel { get; set; } =  false;

    public string? CancelReason { get; set; }

    public bool? IsPorter { get; set; } = false;

    public bool? IsRoundTrip { get; set; } = false;

    public string? Note { get; set; }

    public double? TotalFee { get; set; }

    public DateTime? BookingAt { get; set; }

    public bool? IsReviewOnline { get; set; } = false;

    public bool? IsUserConfirm { get; set; } = false;

    public int? DriverNumber { get; set; }

    public int? PorterNumber { get; set; }

    public int? TruckNumber { get; set; }

    public DateTime? ReviewAt { get; set; }

    public DateTime? EstimatedEndTime { get; set; }

    public bool? IsStaffReviewed { get; set; }

    public bool? IsUpdated { get; set; }

    public bool? IsCredit { get; set; } = false;

    public bool? IsInsurance { get; set; } = false;

    public bool? IsRefunded { get; set; } = false;

    public string? ReasonRefundFailed { get; set; }

    public DateTime? RefundAt { get; set; }

    public double? TotalRefund { get; set; }

    public bool? IsUnchanged { get; set; } = false;

    public string? Shard { get; set; } = DateUtil.GetShardNow();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<BookingTracker> BookingTrackers { get; set; } = new List<BookingTracker>();

    public virtual ICollection<FeeDetail> FeeDetails { get; set; } = new List<FeeDetail>();

    public virtual HouseType? HouseType { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
