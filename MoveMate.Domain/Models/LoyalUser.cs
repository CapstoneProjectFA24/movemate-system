using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class LoyalUser
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<LoyalUserDetail> LoyalUserDetails { get; set; } = new List<LoyalUserDetail>();

    public virtual User? User { get; set; }
}