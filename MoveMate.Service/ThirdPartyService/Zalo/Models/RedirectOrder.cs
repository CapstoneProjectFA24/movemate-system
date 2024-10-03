namespace MoveMate.Service.ThirdPartyService.Zalo.Models;
public class RedirectOrder
{
    public int Appid { get; set; }
    public string Apptransid { get; set; }
    public int Pmcid { get; set; }
    public string Bankcode { get; set; }
    public long Amount { get; set; }
    public long Discountamount { get; set; }
    public int Status { get; set; }
    public string Checksum { get; set; }
}