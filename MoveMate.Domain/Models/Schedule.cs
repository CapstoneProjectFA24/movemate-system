using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public DateOnly? Date { get; set; }

    public virtual ICollection<ScheduleWorking> ScheduleWorkings { get; set; } = new List<ScheduleWorking>();
}
