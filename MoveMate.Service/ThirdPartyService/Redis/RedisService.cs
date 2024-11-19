using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MoveMate.Service.ThirdPartyService.Redis;

public class RedisService : IRedisService
{
    private IDistributedCache _cache;
    private IDatabase _database;
    private readonly IConnectionMultiplexer _redisConnection;
    public RedisService(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
        _cache = cache;
        _redisConnection = connectionMultiplexer;
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
    
    public async Task RemoveKeysWithPatternAsync(string pattern)
    {
        try
        {
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
            
            var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

            if (keys.Length == 0)
            {
                Console.WriteLine("No keys found matching the pattern.");
                return;
            }
            
            var deleteTasks = keys.Select(key => _cache.RemoveAsync(key.ToString())).ToList();

            await Task.WhenAll(deleteTasks);
            Console.WriteLine($"Deleted {deleteTasks.Count} keys matching the pattern '{pattern}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while removing keys with pattern '{pattern}': {ex.Message}");
        }
    }


    // QUEUE
    
    public async Task<bool> KeyExistsQueueAsync(string queueKey)
    {
        try
        {
            bool exists = await _database.KeyExistsAsync(queueKey);
            Console.WriteLine(exists ? $"Key '{queueKey}' exists in Redis." : $"Key '{queueKey}' does not exist in Redis.");
            return exists;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while checking if key '{queueKey}' exists: {ex.Message}");
            return false; // hoặc ném exception tùy vào cách bạn muốn xử lý lỗi
        }
    }

    public async Task EnqueueAsync<T>(string queueKey, T value, TimeSpan? expiry = null)
    {
        var jsonData = JsonSerializer.Serialize(value);
        var expiryTime = expiry ?? TimeSpan.FromHours(20); 

        try
        {
            var existingItems = await _database.ListRangeAsync(queueKey);
            
            foreach (var item in existingItems)
            {
                var existingValue = JsonSerializer.Deserialize<T>(item);
                if (EqualityComparer<T>.Default.Equals(existingValue, value))
                {
                    Console.WriteLine($"Duplicate item found in queue '{queueKey}'. Item not enqueued: {jsonData}");
                    return;
                }
            }
            
            await _database.ListRightPushAsync(queueKey, jsonData);
            //Console.WriteLine($"Successfully enqueued data to queue '{queueKey}': {jsonData}");
            
            var expiryResult = await _database.KeyExpireAsync(queueKey, expiryTime);
            if (expiryResult)
            {
                Console.WriteLine($"Successfully set expiry for queue '{queueKey}' to {expiryTime.TotalHours} hours.");
            }
            else
            {
                Console.WriteLine($"Failed to set expiry for queue '{queueKey}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while enqueueing to queue '{queueKey}': {ex.Message}");
        }
    }
    
    public async Task EnqueueMultipleAsync<T>(string queueKey, IEnumerable<T> values, TimeSpan? expiry = null)
    {
        var expiryTime = expiry ?? TimeSpan.FromHours(20);

        try
        {
            var existingItems = await _database.ListRangeAsync(queueKey);
            var existingValues = existingItems.Select(item => JsonSerializer.Deserialize<T>(item)).ToHashSet();

            foreach (var value in values)
            {
                if (existingValues.Contains(value))
                {
                    Console.WriteLine($"Duplicate item found in queue '{queueKey}'. Item not enqueued: {JsonSerializer.Serialize(value)}");
                    continue; 
                }
                
                await EnqueueAsync(queueKey, value, expiryTime);
            }
            
            var expiryResult = await _database.KeyExpireAsync(queueKey, expiryTime);
            if (expiryResult)
            {
                Console.WriteLine($"Successfully set expiry for queue '{queueKey}' to {expiryTime.TotalHours} hours.");
            }
            else
            {
                Console.WriteLine($"Failed to set expiry for queue '{queueKey}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while enqueueing to queue '{queueKey}': {ex.Message}");
        }
    }
    
    public async Task EnqueueMultipleAsyncV2<T>(string queueKey, IEnumerable<T> values, TimeSpan? expiry = null)
    {
        var expiryTime = expiry ?? TimeSpan.FromHours(20);

        try
        {
            // Lấy tất cả các phần tử hiện có trong hàng đợi Redis
            var existingItems = await _database.ListRangeAsync(queueKey);
            var existingValues = existingItems.Select(item => JsonSerializer.Deserialize<T>(item)).ToHashSet();

            // Chuyển các phần tử không trùng lặp thành RedisValue để chuẩn bị đẩy vào Redis một lần
            var uniqueValues = values
                .Where(value => !existingValues.Contains(value))
                .Select(value => (RedisValue)JsonSerializer.Serialize(value))
                .ToArray();

            if (uniqueValues.Length > 0)
            {
                // Đẩy tất cả các phần tử vào Redis một lần duy nhất
                await _database.ListRightPushAsync(queueKey, uniqueValues);

                // Thiết lập thời gian hết hạn cho key
                var expiryResult = await _database.KeyExpireAsync(queueKey, expiryTime);
                if (expiryResult)
                {
                    Console.WriteLine($"Successfully set expiry for queue '{queueKey}' to {expiryTime.TotalHours} hours.");
                }
                else
                {
                    Console.WriteLine($"Failed to set expiry for queue '{queueKey}'.");
                }
            }
            else
            {
                Console.WriteLine($"No unique items to enqueue in queue '{queueKey}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while enqueueing to queue '{queueKey}': {ex.Message}");
        }
    }

    public async Task<T?> DequeueAsync<T>(string queueKey)
    {
        var length = await _database.ListLengthAsync(queueKey);
        if (length == 0)
        {
            return default;
        }
        var jsonData = await _database.ListLeftPopAsync(queueKey);
        if (jsonData.IsNull)
        {
            return default;
        }
        
        var expiryTime = await _database.KeyTimeToLiveAsync(queueKey);
        if (expiryTime.HasValue)
        {
            await _database.KeyExpireAsync(queueKey, expiryTime.Value);
        }

        return JsonSerializer.Deserialize<T>(jsonData);
    }
    
    public async Task<long> GetQueueLengthAsync(string queueKey)
    {
        return await _database.ListLengthAsync(queueKey);
    }

    public async Task<List<T>> GetQueueItemsAsync<T>(string redisKey)
    {
        var items = new List<T>();

        // Assuming you're using StackExchange.Redis, you can get the items from the Redis list/queue
        var db = _redisConnection.GetDatabase();
        var length = await db.ListLengthAsync(redisKey);

        for (long i = 0; i < length; i++)
        {
            var item = await db.ListGetByIndexAsync(redisKey, i);
            items.Add(JsonConvert.DeserializeObject<T>(item));
        }

        return items;
    }

    public async Task<int> RemoveFromQueueLikeAsync(string queueKey, string searchValue)
    {
        var elements = await _database.ListRangeAsync(queueKey);

        int removedCount = 0;

        foreach (var element in elements)
        {
            var stringValue = element.ToString();

            if (stringValue.Contains(searchValue))
            {
                await _database.ListRemoveAsync(queueKey, element);
                removedCount++;
            }
        }

        return removedCount;
    }

    public async Task<bool> RemoveFromQueueAsync<T>(string queueKey, T value)
    {
        var elements = await _database.ListRangeAsync(queueKey);

        foreach (var element in elements)
        {
            var deserializedValue = JsonSerializer.Deserialize<T>(element);

            if (EqualityComparer<T>.Default.Equals(deserializedValue, value))
            {
                await _database.ListRemoveAsync(queueKey, element);
                return true;
            }
        }

        return false;
    }

    // EnqueueWithExpiry
    public async Task EnqueueWithExpiryAsync<T>(string queueKey, T value, TimeSpan? expiry = null)
    {
        var actualExpiry = expiry ?? TimeSpan.FromHours(20);

        var jsonData = JsonSerializer.Serialize(value);
        var expiryTime = DateTimeOffset.Now.Add(actualExpiry).ToUnixTimeSeconds();

        await _database.SortedSetAddAsync(queueKey, jsonData, expiryTime);
    }

    public async Task<T?> DequeueWithExpiryAsync<T>(string queueKey)
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();

        var values = await _database.SortedSetRangeByScoreAsync(queueKey, 0, now, Exclude.None, Order.Ascending, 0, 1);

        if (values.Length == 0) return default;

        var jsonData = values[0];
        await _database.SortedSetRemoveAsync(queueKey, jsonData); // Xóa phần tử sau khi lấy

        return JsonSerializer.Deserialize<T>(jsonData);
    }
    
    public async Task<long> CheckQueueCountAsync(string queueKey)
    {
        try
        {
            long count = await _database.ListLengthAsync(queueKey);
            Console.WriteLine($"Queue '{queueKey}' hiện có {count} phần tử.");
            return count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Đã xảy ra lỗi khi kiểm tra số lượng phần tử trong queue '{queueKey}': {ex.Message}");
            return 0;
        }
    }

}