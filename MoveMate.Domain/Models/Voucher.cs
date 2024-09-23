using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? BookingId { get; set; }

    public int? PromotionCategoryId { get; set; }

    public double? Price { get; set; }

    public string? Code { get; set; }

    public bool? IsActived { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual PromotionCategory? PromotionCategory { get; set; }

    public virtual User? User { get; set; }
}
