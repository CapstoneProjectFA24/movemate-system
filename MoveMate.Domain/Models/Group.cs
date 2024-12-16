using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Group
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActived { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DurationTimeActived { get; set; }

    public virtual ICollection<ScheduleWorking> ScheduleWorkings { get; set; } = new List<ScheduleWorking>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
