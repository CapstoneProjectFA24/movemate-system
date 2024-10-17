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

    public DateOnly? ExpirationDate { get; set; }

    public DateOnly? RefreshExpirationDate { get; set; }

    public bool? IsMobile { get; set; }

    public virtual User? User { get; set; }
}