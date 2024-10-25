using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ScheduleBookingDetail
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public int? UserId { get; set; }

    public int? ScheduleBookingId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? DurationTime { get; set; }

    public double? Amount { get; set; }

    public string? Type { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ScheduleBooking? ScheduleBooking { get; set; }

    public virtual User? User { get; set; }
}
