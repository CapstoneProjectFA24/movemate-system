using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class User
{
    public int Id { get; set; }

    public int? ScheduleId { get; set; }

    public int? RoleId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? AvatarUrl { get; set; }

    public DateOnly? Dob { get; set; }

    public bool? IsBanned { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ModifiedVersion { get; set; }

    public bool? IsDriver { get; set; }

    public string? CodeIntroduce { get; set; }

    public string? NumberIntroduce { get; set; }

    public bool? IsInitUser { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<BookingStaffDaily> BookingStaffDailies { get; set; } = new List<BookingStaffDaily>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PromotionDetail> PromotionDetails { get; set; } = new List<PromotionDetail>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; } = new List<ScheduleDetail>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    public virtual ICollection<TripAccuracy> TripAccuracies { get; set; } = new List<TripAccuracy>();

    public virtual ICollection<Truck> Trucks { get; set; } = new List<Truck>();

    public virtual ICollection<UserInfo> UserInfos { get; set; } = new List<UserInfo>();

    public virtual Wallet? Wallet { get; set; }
}
