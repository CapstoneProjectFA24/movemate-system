﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ScheduleBooking
{
    public int Id { get; set; }

    public bool? IsActived { get; set; }

    public string? Shard { get; set; }

    public virtual ICollection<ScheduleBookingDetail> ScheduleBookingDetails { get; set; } = new List<ScheduleBookingDetail>();
}