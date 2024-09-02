using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class BookingItem
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? BookingId { get; set; }

    public string? Status { get; set; }

    public int? Quantity { get; set; }

    public string? EstimatedWeight { get; set; }

    public string? EstimatedLenght { get; set; }

    public string? EstimatedWidth { get; set; }

    public string? EstimatedHeight { get; set; }

    public string? EstimatedVolume { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Item? Item { get; set; }
}
