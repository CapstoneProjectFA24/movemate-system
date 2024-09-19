using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class FeeSetting
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Amount { get; set; }

    public bool? IsActived { get; set; }
    
    public bool? IsDefault { get; set; }

    public string? Type { get; set; }

    public string? Unit { get; set; }

    public double? RangeMin { get; set; }

    public double? RangeMax { get; set; }

    public int? HouseTypeSettingId { get; set; }

    public int? TruckCategoryId { get; set; }

    public int? ServiceId { get; set; }

    public int? DiscountRate { get; set; }

    public virtual ICollection<FeeDetail> FeeDetails { get; set; } = new List<FeeDetail>();

    public virtual HouseTypeSetting? HouseTypeSetting { get; set; }

    public virtual Service? Service { get; set; }

    public virtual TruckCategory? TruckCategory { get; set; }
}
