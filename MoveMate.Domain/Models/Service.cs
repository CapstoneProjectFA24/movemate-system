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

    public int? DiscountRate { get; set; }

    public double? Amount { get; set; }

    public int? ParentServiceId { get; set; }

    public virtual ICollection<Service> InverseParentService { get; set; } = new List<Service>();

    public virtual Service? ParentService { get; set; }

    public virtual ICollection<ServiceDetail> ServiceDetails { get; set; } = new List<ServiceDetail>();
}
