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

    public string? Type { get; set; }

    public string? Unit { get; set; }

    public double? RangeMin { get; set; }

    public double? RangeMax { get; set; }

    public bool? IsMin { get; set; }

    public bool? IsMax { get; set; }
}
