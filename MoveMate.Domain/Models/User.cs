using MoveMate.Domain.Utils;
using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class User
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? Dob { get; set; }

    public bool? IsBanned { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ModifiedVersion { get; set; }

    public bool? IsInitUsed { get; set; }

    public bool? IsDriver { get; set; }

    public string? CodeIntroduce { get; set; }

    public string? NumberIntroduce { get; set; }

    public int? GroupId { get; set; }

    public string? Shard { get; set; } = DateUtil.GetShardNow();

    public bool? IsAccepted { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Group? Group { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role? Role { get; set; }

    public virtual Truck? Truck { get; set; }

    public virtual ICollection<UserInfo> UserInfos { get; set; } = new List<UserInfo>();

    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();

    public virtual Wallet? Wallet { get; set; }

    public virtual ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
