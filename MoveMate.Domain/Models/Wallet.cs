using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public double? Balance { get; set; }

    public int? Tier { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsLocked { get; set; }

    public string? LockReason { get; set; }

    public double? LockAmount { get; set; }

    public string? Type { get; set; }

    public double? FixedSalary { get; set; }

    public string? BankNumber { get; set; }

    public string? BankName { get; set; }

    public string? ExpirdAt { get; set; }

    public string? CardHolderName { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
