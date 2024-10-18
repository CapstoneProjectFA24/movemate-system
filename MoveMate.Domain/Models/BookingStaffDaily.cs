﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class BookingStaffDaily
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public string? AddressCurrent { get; set; }

    public bool? IsActived { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DurationTimeActived { get; set; }

    public virtual User? User { get; set; }
}
