using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class HouseType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<FeeSetting> FeeSettings { get; set; } = new List<FeeSetting>();

    public virtual ICollection<HouseTypeSetting> HouseTypeSettings { get; set; } = new List<HouseTypeSetting>();
}