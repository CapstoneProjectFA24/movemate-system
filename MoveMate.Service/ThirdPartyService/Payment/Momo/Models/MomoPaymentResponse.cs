namespace MoveMate.Service.ThirdPartyService.Payment.Momo.Models
;

public class MomoPaymentResponse
{
    public string PartnerCode { get; set; } = default!;

    public string RequestId { get; set; } = default!;

    public string OrderId { get; set; } = default!;

    public long Amount { get; set; } = default!;

    public long ResponseTime { get; set; } = default!;

    public string Message { get; set; } = default!;

    public int ResultCode { get; set; } = default!;

    public string PayUrl { get; set; } = default!;

    public string Deeplink { get; set; } = default!;

    public string QrCodeUrl { get; set; } = default!;

    public string DeeplinkMiniApp { get; set; } = default!;

    public string Signature { get; set; } = default!;
}