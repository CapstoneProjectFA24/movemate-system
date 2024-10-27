using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class TrackerSource
{
    public int Id { get; set; }

    public int? BookingTrackerId { get; set; }

    public string? ResourceUrl { get; set; }

    public string? ResourceCode { get; set; }

    public string? Type { get; set; }

    public bool? IsDeleted { get; set; } 

    public virtual BookingTracker? BookingTracker { get; set; }
}
