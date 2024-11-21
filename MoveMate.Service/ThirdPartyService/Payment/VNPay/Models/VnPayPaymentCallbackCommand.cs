namespace MoveMate.Service.ThirdPartyService.Payment.VNPay.Models;

public class VnPayPaymentCallbackCommand
{
    public string? vnp_TransactionStatus { get; set; } = default!;

    public string? vnp_TmnCode { get; set; } = default!;
    public long? vnp_Amount { get; set; } = default!;

    public string? vnp_BankCode { get; set; } = default!;

    public string? vnp_BankTranNo { get; set; } = default!;

    public string? vnp_CardType { get; set; } = default!;

    public string? vnp_PayDate { get; set; } = default!;

    public string? vnp_SecureHashType { get; set; } = default!;

    public string? vnp_TransactionNo { get; set; } = default!;

    public string? vnp_TxnRef { get; set; } = default!;

    public string? vnp_OrderInfo { get; set; } = default!;
    public string? vnp_OrderType { get; set; } = default!;

    public string? vnp_SecureHash { get; set; } = default!;

    public string? vnp_ResponseCode { get; set; } = default!;
    public string returnUrl { get; set; } = default!;
    public int userId { get; set; } = default!;

    public string category { get; set; } = default!;

    public bool IsSuccess => "00".Equals(vnp_ResponseCode);
}