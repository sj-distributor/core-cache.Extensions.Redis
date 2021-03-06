using System;
using System.Threading.Tasks;
using NetCoreCache.Extendsions.Redis.Driver;
using Xunit;

namespace UnitTests;

[Collection("Sequential")]
public class RedisCacheTests
{
    private RedisCache _redisClient;

    public RedisCacheTests()
    {
        _redisClient = new RedisCache("server=localhost:6379;timeout=5000;MaxMessageSize=1024000;Expire=3600");
    }

    [Theory]
    [InlineData("anson", "18", "18")]
    [InlineData("anson1", "19", "19")]
    public async void TestRedisCacheCanSet(string key, string value, string result)
    {
        await _redisClient.Set(key, value);
        var s = await _redisClient.Get(key);
        Assert.Equal(s, result);
    }

    [Theory]
    [InlineData("key1", "18", "", 1)]
    [InlineData("key2", "19", "", 1)]
    public async void TestRedisCacheCanSetTimeout(string key, string value, string result, long expire = 0)
    {
        await _redisClient.Set(key, value, expire);

        await Task.Delay(TimeSpan.FromSeconds(2));

        var s = await _redisClient.Get(key);
        Assert.Equal(s, result);
    }

    [Theory]
    [InlineData("anson", "18", "")]
    [InlineData("anson1", "19", "")]
    public async void TestRedisCacheCanDelete(string key, string value, string result)
    {
        await _redisClient.Set(key, value);
        await _redisClient.Delete(key);
        var s = await _redisClient.Get(key);
        Assert.Equal(s, result);
    }

    [Theory]
    [InlineData("anson1111", "18", "")]
    [InlineData("anson2222", "19", "")]
    public async void TestRedisCacheCanDeleteByPattern(string key, string value, string result)
    {
        await _redisClient.Set(key, value);
        await _redisClient.Delete("anson*");
        var s = await _redisClient.Get(key);
        Assert.Equal(s, result);
    }
}