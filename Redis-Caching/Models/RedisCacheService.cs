using StackExchange.Redis;

using System.Text.Json;

public class RedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task SetCacheValueAsync(string key, object value, TimeSpan? expiration = null)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, jsonData, expiration);
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        var jsonData = await _db.StringGetAsync(key);
        return jsonData.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(jsonData);
    }

    public async Task RemoveCacheValueAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
