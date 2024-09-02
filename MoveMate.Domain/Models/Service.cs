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

    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();
}
