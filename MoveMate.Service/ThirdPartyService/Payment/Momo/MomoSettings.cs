namespace MoveMate.Service.ThirdPartyService.Payment.Momo;

public class MomoSettings
{
    public string PartnerCode { get; set; } = default!;

    public string AccessKey { get; set; } = default!;

    public string SecretKey { get; set; } = default!;

    public string PaymentEndpoint { get; set; } = default!;

    public string IpnUrl { get; set; } = default!;

    public string RedirectUrl { get; set; } = string.Empty;
}