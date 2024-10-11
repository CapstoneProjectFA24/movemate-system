namespace MoveMate.Service.ThirdPartyService.Redis;

public interface IRedisService
{
    T? GetData<T>(string key);
    
    void SetData<T>(string key, T value, TimeSpan? expiry = null);

    void RemoveData(string key);
    
    Task<T?> GetDataAsync<T>(string key);
    
    Task SetDataAsync<T>(string key, T value, TimeSpan? expiry = null);

    Task RemoveDataAsync(string key);

    Task<bool> KeyExistsAsync(string key);

}