using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public bool? IsActived { get; set; }

    public bool? IsDefault { get; set; }

    public int? WorkOvertime { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? ScheduleDetailsId { get; set; }

    public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; } = new List<ScheduleDetail>();
}
