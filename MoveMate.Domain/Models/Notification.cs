using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? SentFrom { get; set; }

    public string? Receive { get; set; }

    public string? FCMToken { get; set; }

    public string? DeviceId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Topic { get; set; }

    public virtual User? User { get; set; }
}
