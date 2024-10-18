using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Token
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Token1 { get; set; }

    public string? RefreshToken { get; set; }

    public string? TokenType { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public DateTime? RefreshExpirationDate { get; set; }

    public bool? IsMobile { get; set; }

    public virtual User? User { get; set; }
}
