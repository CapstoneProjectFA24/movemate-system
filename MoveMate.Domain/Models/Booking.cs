using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public double? Deposit { get; set; }

    public string? Status { get; set; }

    public string? PickupAddress { get; set; }

    public string? PickupPoint { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? DeliveryPoint { get; set; }

    public bool? IsUseBox { get; set; }

    public string? BoxType { get; set; }

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

    public string? Bonus { get; set; }

    public string? TypeBooking { get; set; }

    public string? EstimatedAcreage { get; set; }

    public string? RoomNumber { get; set; }

    public string? FloorsNumber { get; set; }

    public bool? IsManyItems { get; set; }

    public string? EstimatedTotalWeight { get; set; }

    public bool? IsCancel { get; set; }

    public string? CancelReason { get; set; }

    public string? EstimatedWeight { get; set; }

    public string? EstimatedHeight { get; set; }

    public string? EstimatedWidth { get; set; }

    public string? EstimatedLength { get; set; }

    public string? EstimatedVolume { get; set; }

    public bool? IsPorter { get; set; }

    public bool? IsRoundTrip { get; set; }

    public string? Note { get; set; }

    public double? TotalFee { get; set; }

    public string? FeeInfo { get; set; }

    public virtual ICollection<AchievementDetail> AchievementDetails { get; set; } = new List<AchievementDetail>();

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual ICollection<BookingTracker> BookingTrackers { get; set; } = new List<BookingTracker>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<PromotionDetail> PromotionDetails { get; set; } = new List<PromotionDetail>();

    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();

    public virtual User? User { get; set; }
}
