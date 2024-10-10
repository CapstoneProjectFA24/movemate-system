namespace MoveMate.Service.ThirdPartyService.Momo.Models
;

public class MomoPaymentRequest
{
    public string OrderInfo { get; set; } = default!;

    public string PartnerCode { get; set; } = default!;

    public string RedirectUrl { get; set; } = default!;

    public string IpnUrl { get; set; } = default!;

    public long Amount { get; set; }

    public string OrderId { get; set; } = default!;

    public string ReferenceId { get; set; } = default!;

    public string RequestId { get; set; } = default!;

    public string ExtraData { get; set; } = default!;

    public string? PartnerName { get; set; } = default!;

    public string? StoreId { get; set; } = default!;

    public string RequestType { get; set; } = default!;

    public string? OrderGroupId { get; set; } = default!;

    public bool AutoCapture { get; set; }

    public string Lang { get; set; } = default!;

    public string Signature { get; set; } = default!;
}
