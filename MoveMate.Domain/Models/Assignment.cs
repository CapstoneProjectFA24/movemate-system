using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Assignment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? BookingDetailsId { get; set; }

    public int? TruckId { get; set; }

    public int? BookingId { get; set; }

    public int? ScheduleBookingId { get; set; }

    public string? Status { get; set; }

    public double? Price { get; set; }

    public string? StaffType { get; set; }

    public bool? IsResponsible { get; set; }

    public string? AddressCurrent { get; set; }

    public string? FailedReason { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? DurationTime { get; set; }

    public bool? IsRoundTripCompleted { get; set; }

    public double? Bonus { get; set; }

    public double? StarReview { get; set; }
    public string? Review { get; set; }
    public bool? IsChanged { get; set; } = false;

    public virtual Booking? Booking { get; set; }

    public virtual BookingDetail? BookingDetails { get; set; }

    public virtual ScheduleBooking? ScheduleBooking { get; set; }

    public virtual Truck? Truck { get; set; }

    public virtual User? User { get; set; }
}
