using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class UserInfo
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Type { get; set; }

    public string? ImageUrl { get; set; }

    public string? Value { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User? User { get; set; }
}
