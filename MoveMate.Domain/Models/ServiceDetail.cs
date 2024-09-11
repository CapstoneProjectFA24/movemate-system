﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ServiceDetail
{
    public int Id { get; set; }

    public int? ServiceId { get; set; }

    public int? BookingId { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public bool? IsQuantity { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Service? Service { get; set; }
}