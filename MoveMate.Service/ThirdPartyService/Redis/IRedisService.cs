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

    Task RemoveKeysWithPatternAsync(string pattern);
    // Queue
    public Task<bool> KeyExistsQueueAsync(string queueKey);

    Task EnqueueAsync<T>(string queueKey, T value, TimeSpan? expiry = null);

    Task<T?> DequeueAsync<T>(string queueKey);

    Task<long> GetQueueLengthAsync(string queueKey);

    Task<int> RemoveFromQueueLikeAsync(string queueKey, string searchValue);

    Task<bool> RemoveFromQueueAsync<T>(string queueKey, T value);

    Task EnqueueMultipleAsync<T>(string queueKey, IEnumerable<T> values, TimeSpan? expiry = null);
    Task EnqueueMultipleAsyncV2<T>(string queueKey, IEnumerable<T> values, TimeSpan? expiry = null);

    Task EnqueueWithExpiryAsync<T>(string queueKey, T value, TimeSpan? expiry = null);

    Task<T?> DequeueWithExpiryAsync<T>(string queueKey);

    Task<long> CheckQueueCountAsync(string queueKey);
}