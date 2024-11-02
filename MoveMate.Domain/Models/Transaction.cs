using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int? PaymentId { get; set; }

    public int? WalletId { get; set; }

    public string? Resource { get; set; }

    public double? Amount { get; set; }

    public string? Status { get; set; }

    public string? Substance { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionCode { get; set; }

    public string? TransactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
    public bool? IsCredit { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
