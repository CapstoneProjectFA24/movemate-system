﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class BookingDetail
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? BookingId { get; set; }

    public string? Status { get; set; }

    public double? Price { get; set; }

    public string? StaffType { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? User { get; set; }
}
