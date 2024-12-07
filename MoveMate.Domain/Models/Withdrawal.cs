using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Withdrawal
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? WalletId { get; set; }

    public double? BalanceBefore { get; set; }

    public double? BalanceAfter { get; set; }

    public DateTime? Date { get; set; }

    public string? BankName { get; set; }

    public string? BankNumber { get; set; }

    public bool? IsSuccess { get; set; }

    public bool? IsCancel { get; set; }

    public string? CancelReason { get; set; }

    public double? Amount { get; set; }

    public virtual User? User { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
