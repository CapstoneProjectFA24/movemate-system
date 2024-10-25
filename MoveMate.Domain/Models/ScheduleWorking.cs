using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ScheduleWorking
{
    public int Id { get; set; }

    public string? Status { get; set; }

    public bool? IsActived { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DurationTimeActived { get; set; }

    public string? Type { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<BookingStaffDaily> BookingStaffDailies { get; set; } = new List<BookingStaffDaily>();
}
