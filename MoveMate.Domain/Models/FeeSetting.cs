﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class FeeSetting
{
    public int Id { get; set; }

    public int? ServiceId { get; set; }

    public int? HouseTypeId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? Amount { get; set; }

    public bool? IsActived { get; set; }

    public string? Type { get; set; }

    public string? Unit { get; set; }

    public double? RangeMin { get; set; }

    public double? RangeMax { get; set; }

    public string? DiscountRate { get; set; }

    public double? FloorPercentage { get; set; }

    public virtual ICollection<FeeDetail> FeeDetails { get; set; } = new List<FeeDetail>();

    public virtual HouseType? HouseType { get; set; }

    public virtual Service? Service { get; set; }
}
