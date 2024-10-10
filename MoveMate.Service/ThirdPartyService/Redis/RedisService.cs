using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MoveMate.Service.ThirdPartyService.Redis;

public class RedisService:IRedisService
{
    private IDistributedCache _cache;

    public RedisService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public T? GetData<T>(string key)
    {
        var data = _cache.GetString(key);

        if (data is null)
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(data)!;
    }

    public void SetData<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
        };
        _cache.SetString(key, JsonSerializer.Serialize(value), options);
    }
    
    public void RemoveData(string key)
    {
        _cache.Remove(key);
    }
    
    public async Task<T?> GetDataAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);

        if (data is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetDataAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
        };

        var jsonData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task RemoveDataAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
    
    public async Task<bool> KeyExistsAsync(string key)
    {
        var data = await _cache.GetStringAsync(key);
        return data != null;
    }


}