using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class HouseType
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<HouseTypeSetting> HouseTypeSettings { get; set; } = new List<HouseTypeSetting>();
}
