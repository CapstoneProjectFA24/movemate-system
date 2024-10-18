using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class BookingTracker
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public string? Time { get; set; }

    public string? Type { get; set; }

    public string? Location { get; set; }

    public string? Point { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<TrackerSource> TrackerSources { get; set; } = new List<TrackerSource>();
}
