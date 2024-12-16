using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class PromotionCategory
{
    public int Id { get; set; }

    public bool? IsPublic { get; set; } = true;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? DiscountRate { get; set; }

    public double? DiscountMax { get; set; }

    public double? RequireMin { get; set; }

    public double? DiscountMin { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public int? Quantity { get; set; }

    public DateTime? StartBookingTime { get; set; }

    public DateTime? EndBookingTime { get; set; }

    public bool? IsInfinite { get; set; }

    public int? ServiceId { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Shard { get; set; }

    public double? Amount { get; set; }

    public virtual Service? Service { get; set; }

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
