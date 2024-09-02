using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Statistical
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Type { get; set; }

    public string? Shard { get; set; }

    public string? Week { get; set; }

    public DateOnly? Date { get; set; }

    public double? Total { get; set; }

    public int? Tier { get; set; }

    public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; } = new List<ScheduleDetail>();

    public virtual User? User { get; set; }
}
