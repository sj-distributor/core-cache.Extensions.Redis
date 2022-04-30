using Core.Driver;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCache.Extendsions.Redis.Extendsions;

namespace NetCoreCache.Extendsions.Redis;

public static class NetCoreCacheExtendsionsRedis
{
    public static void AddNetCoreCacheRedis(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ICacheClient>(new RedisCache(connectionString));
    }
}