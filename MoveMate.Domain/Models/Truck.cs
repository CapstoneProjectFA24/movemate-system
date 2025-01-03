﻿using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Truck
{
    public int Id { get; set; }

    public int? TruckCategoryId { get; set; }

    public string? Model { get; set; }

    public string? NumberPlate { get; set; }

    public double? Capacity { get; set; }

    public bool? IsAvailable { get; set; } =  false;

    public string? Brand { get; set; }

    public string? Color { get; set; }

    public bool? IsInsurrance { get; set; } = true;

    public int? UserId { get; set; }

    public bool? IsDeleted { get; set; } = false;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual TruckCategory? TruckCategory { get; set; }

    public virtual ICollection<TruckImg> TruckImgs { get; set; } = new List<TruckImg>();

    public virtual User? User { get; set; }
}
