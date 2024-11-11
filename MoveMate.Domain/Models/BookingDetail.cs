using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class BookingDetail
{
    public int Id { get; set; }

    public int? ServiceId { get; set; }

    public int? BookingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public bool? IsRoundTripCompleted { get; set; }

    public string? FailReason { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Booking? Booking { get; set; }

    public virtual Service? Service { get; set; }
}
