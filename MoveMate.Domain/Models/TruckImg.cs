using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class TruckImg
{
    public int Id { get; set; }

    public int? TruckId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageCode { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Truck? Truck { get; set; }
}
