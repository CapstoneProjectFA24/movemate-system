using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Achievement
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<AchievementDetail> AchievementDetails { get; set; } = new List<AchievementDetail>();

    public virtual User? User { get; set; }
}
