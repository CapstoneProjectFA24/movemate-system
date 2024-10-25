using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Service
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActived { get; set; }

    public int? Tier { get; set; }

    public string? ImageUrl { get; set; }

    public double? DiscountRate { get; set; }

    public double? Amount { get; set; }

    public int? ParentServiceId { get; set; }

    public string? Type { get; set; }

    public bool? IsQuantity { get; set; }

    public int? QuantityMax { get; set; }

    public int? TruckCategoryId { get; set; }

    public int? PromotionCategoryId { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<FeeSetting> FeeSettings { get; set; } = new List<FeeSetting>();

    public virtual ICollection<Service> InverseParentService { get; set; } = new List<Service>();

    public virtual Service? ParentService { get; set; }

    public virtual PromotionCategory? PromotionCategory { get; set; }

    public virtual TruckCategory? TruckCategory { get; set; }
}
