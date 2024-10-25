using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class HolidaySetting
{
    public int Id { get; set; }

    public DateOnly? Day { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
