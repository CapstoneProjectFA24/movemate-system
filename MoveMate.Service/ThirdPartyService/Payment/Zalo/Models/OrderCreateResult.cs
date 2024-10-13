namespace MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;

public class ZaloPayOrderResult
{
    public int ReturnCode { get; set; }
    public string ReturnMessage { get; set; }
    public int SubReturnCode { get; set; }
    public string SubReturnMessage { get; set; }
    public string ZpTransToken { get; set; }
    public string OrderUrl { get; set; }
    public string OrderToken { get; set; }
}