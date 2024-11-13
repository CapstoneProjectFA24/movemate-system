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

    public TimeOnly? StartDate { get; set; }

    public TimeOnly? EndDate { get; set; }

    public int? GroupId { get; set; }

    public TimeOnly? ExtentStartDate { get; set; }

    public TimeOnly? ExtentEndDate { get; set; }

    public virtual Group? Group { get; set; }
}
