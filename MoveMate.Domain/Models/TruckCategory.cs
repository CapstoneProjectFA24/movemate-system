using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class TruckCategory
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public double? MaxLoad { get; set; }

    public string? Description { get; set; }

    public string? ImgUrl { get; set; }

    public string? EstimatedLength { get; set; }

    public string? EstimatedWidth { get; set; }

    public string? EstimatedHeight { get; set; }

    public string? Summarize { get; set; }

    public double? Price { get; set; }

    public int? TotalTrips { get; set; }

    public virtual ICollection<FeeSetting> FeeSettings { get; set; } = new List<FeeSetting>();

    public virtual ICollection<HouseTypeSetting> HouseTypeSettings { get; set; } = new List<HouseTypeSetting>();

    public virtual ICollection<Truck> Trucks { get; set; } = new List<Truck>();
}
