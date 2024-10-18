using System;
using System.Collections.Generic;

namespace MoveMate.Domain.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public string? BankCode { get; set; }

    public string? BankTransNo { get; set; }

    public string? CardType { get; set; }

    public double? Amount { get; set; }

    public string? Token { get; set; }

    public string? ResponseCode { get; set; }

    public bool? Success { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
