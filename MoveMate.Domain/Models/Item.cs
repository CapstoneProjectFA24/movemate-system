using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Item
{
    public int Id { get; set; }

    public int? ItemCategoryId { get; set; }

    public double? Price { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Description { get; set; }

    public int? Tier { get; set; }

    public string? ImgUrl { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual ItemCategory? ItemCategory { get; set; }
}
