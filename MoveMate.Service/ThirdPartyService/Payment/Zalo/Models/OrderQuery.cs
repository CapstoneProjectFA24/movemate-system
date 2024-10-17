namespace MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;

public class OrderQuery : IBaseFormRequest
{
    public int AppId { set; get; }
    public string AppTransId { set; get; }
    public string Mac { set; get; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "app_id", AppId.ToString() },
            { "app_trans_id", AppTransId },
            { "mac", Mac }
        };
    }
}