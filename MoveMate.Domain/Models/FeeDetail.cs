using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class FeeDetail
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public int? FeeSettingId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Amount { get; set; }

    public int? Quantity { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual FeeSetting? FeeSetting { get; set; }
}