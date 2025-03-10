﻿using System.Text.Json.Serialization;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class BookingDetailRequest
{
    [JsonIgnore]
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int? Quantity { get; set; }
}