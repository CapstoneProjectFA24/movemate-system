using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class LoyalUserDetail
{
    public int Id { get; set; }

    public int? LoyalUserId { get; set; }

    public int? BookingId { get; set; }

    public int? Quantity { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual LoyalUser? LoyalUser { get; set; }
}
