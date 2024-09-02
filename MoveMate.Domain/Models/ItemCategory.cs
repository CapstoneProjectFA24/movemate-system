using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class ItemCategory
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ImgUrl { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
