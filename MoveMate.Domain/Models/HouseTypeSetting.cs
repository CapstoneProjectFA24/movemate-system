using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class HouseTypeSetting
{
    public int Id { get; set; }

    public int? HouseTypeId { get; set; }

    public int? TruckCategoryId { get; set; }

    public int? NumberOfFloors { get; set; }

    public int? NumberOfRooms { get; set; }

    public int? NumberOfTrucks { get; set; }

    public virtual HouseType? HouseType { get; set; }

    public virtual TruckCategory? TruckCategory { get; set; }
}
