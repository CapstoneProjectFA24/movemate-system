using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ScheduleDetail
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public int? ScheduleId { get; set; }

    public int? StatisticalId { get; set; }

    public DateOnly? WorkingDays { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? DurationTime { get; set; }

    public double? Amount { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public virtual Statistical? Statistical { get; set; }
}
