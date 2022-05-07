using Core.Driver;

namespace NetCoreCache.Extendsions.Redis.Driver;

public class RedisCache : ICacheClient
{
    private bool _canGetRedisClient = false;

    private readonly NewLife.Caching.Redis _redisClient;

    public NewLife.Caching.Redis? GetRedisClient()
    {
        return _canGetRedisClient ? _redisClient : null;
    }

    public RedisCache(string connectionString, bool canGetRedisClient = false)
    {
        _canGetRedisClient = canGetRedisClient;
        _redisClient = new NewLife.Caching.Redis();
        _redisClient.Init(connectionString);
    }

    public ValueTask Set(string key, string value, long expire = 0)
    {
        var hasKey = _redisClient.ContainsKey(key);
        if (hasKey) return ValueTask.CompletedTask;
        if (expire > 0)
        {
            _redisClient.Add(key, value, (int)(expire));
        }
        else
        {
            _redisClient.Add(key, value);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<string> Get(string key)
    {
        var result = _redisClient.Get<string>(key);
        return ValueTask.FromResult(string.IsNullOrEmpty(result) ? "" : result);
    }

    public ValueTask Delete(string key)
    {
        if (key.Contains('*'))
        {
            if (key.First() == '*')
            {
                key = key.Substring(1, key.Length - 1);
            }
            else if (key.Last() == '*')
            {
                key = key[..^1];
            }

            var list = _redisClient.Keys.Where(x => x.Contains(key)).ToArray();
            _redisClient.Remove(list);
        }
        else
        {
            _redisClient.Remove(key);
        }

        return ValueTask.CompletedTask;
    }
}