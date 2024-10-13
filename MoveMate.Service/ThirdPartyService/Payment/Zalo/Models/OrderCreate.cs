namespace MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;

public class ZaloPayOrderCreate : IBaseFormRequest
{
    public int AppId { get; set; }
    public string AppUser { get; set; }
    public string AppTransId { get; set; }
    public long AppTime { get; set; }
    public long Amount { get; set; }
    public string Item { get; set; }
    public string Description { get; set; }
    public string EmbedData { get; set; }
    public string BankCode { get; set; }
    public string Mac { get; set; }
    public string CallbackUrl { get; set; }
    public string DeviceInfo { get; set; }
    public string SubAppId { get; set; }
    public string Title { get; set; }
    public string Currency { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "app_id", AppId.ToString() },
            { "app_user", AppUser },
            { "app_trans_id", AppTransId },
            { "app_time", AppTime.ToString() },
            { "amount", Amount.ToString() },
            { "item", Item },
            { "description", Description },
            { "embed_data", EmbedData },
            { "bank_code", BankCode },
            { "mac", Mac },
            { "callback_url", CallbackUrl },
            { "device_info", DeviceInfo },
            { "sub_app_id", SubAppId },
            { "Title", Title },
            { "currency", Currency },
            { "phone", Phone },
            { "email", Email },
            { "address", Address },
        };
    }
}