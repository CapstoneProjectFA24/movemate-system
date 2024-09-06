using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class TripAccuracy
{
    public int Id { get; set; }

    public int? TotalTrip { get; set; }

    public int? TotalApprovedTrip { get; set; }

    public int? TotalTripReject { get; set; }

    public int? TotalTripMiss { get; set; }

    public int? TotalApprovedTripInPeriod { get; set; }

    public int? TotalTripInPeriod { get; set; }

    public int? TotalTripRejectInPeriod { get; set; }

    public int? TotalTripMissInPeriod { get; set; }

    public string? Shard { get; set; }

    public double? AccuracyInPeriod { get; set; }

    public double? AccuracyInPre { get; set; }

    public double? TotalIncome { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
