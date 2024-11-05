namespace MoveMate.Service.ThirdPartyService.Firebase.Notification;

public interface IJsonSerializer
{
    string Serialize(object obj);
    TObject Deserialize<TObject>(string json);
}