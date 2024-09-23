using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class LoyalUserSetting
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? Tier { get; set; }

    public string? AwardWinningHook { get; set; }

    public bool? IsActived { get; set; }
}
