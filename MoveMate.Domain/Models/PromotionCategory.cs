using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class PromotionCategory
{
    public int Id { get; set; }

    public bool? IsPublic { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? DiscountRate { get; set; }

    public double? DiscountMax { get; set; }

    public double? DiscountPrice { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public int? Quantity { get; set; }

    public DateTime? StartBookingTime { get; set; }

    public DateTime? EndBookingTime { get; set; }

    public bool? IsInfinite { get; set; }

    public virtual ICollection<PromotionDetail> PromotionDetails { get; set; } = new List<PromotionDetail>();
}
