using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class AchievementDetail
{
    public int Id { get; set; }

    public int? AchievementId { get; set; }

    public int? BookingId { get; set; }

    public int? Quantity { get; set; }

    public virtual Achievement? Achievement { get; set; }

    public virtual Booking? Booking { get; set; }
}
