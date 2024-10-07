namespace MoveMate.Service.ThirdPartyService.Zalo.Models;

public class OrderQueryResult
{
    public int ReturnCode { get; set; }
    public string ReturnMessage { get; set; }
    public int SubReturnCode { get; set; }
    public string SubReturnMessage { get; set; }
    public bool IsProcessing { get; set; }
    public long Amount { get; set; }
    public long ZpTransId { get; set; }
}