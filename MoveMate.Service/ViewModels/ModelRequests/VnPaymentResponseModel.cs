using MoveMate.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class VnPaymentResponseModel
{
    public bool Success { get; set; }
    public int WalletId { get; set; }
    public int OrderId { get; set; }
    public string BankCode { get; set; }
    public string BankTranNo { get; set; }
    public string CardType { get; set; }
    public float Amount { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
}

public class RechagreResponseModel
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public string BankCode { get; set; }
    public string BankTranNo { get; set; }
    public string CardType { get; set; }
    public float Amount { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
}

public class VnPaymentRequestModel
{
    //public int AuctionId { get; set; }
    //public float Amount { get; set; }
    public int OrderId { get; set; }

    public DateTime CreatedDate
    {
        get => DateTime.Now;
        init { }
    }
}

public class VnPaymentRecharge
{
    public float Amount { get; set; }
    // public int UserId { get; set; }
    //public int WalletId { get; set; }
}

public record PaymentRequestCreateModel
{
    public int? WalletId { get; init; }
    public FromToWallet From { get; init; }
    public FromToWallet To { get; init; }
    public float Amount { get; init; }
    public PaymentMethod Type { get; init; }
}