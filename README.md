# NetCoreCache.Extensions.Redis

[![Build Status](https://github.com/sj-distributor/core-cache.Extensions.Redis/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/sj-distributor/core-cache.Extensions.Redis/actions?query=branch%3Amain)
[![codecov](https://codecov.io/gh/sj-distributor/core-cache.Extensions.Redis/branch/main/graph/badge.svg?token=XV3W873RGV)](https://codecov.io/gh/sj-distributor/core-cache.Extensions.Redis)
[![NuGet version (NetCoreCache)](https://img.shields.io/nuget/v/NetCoreCacheRedis.svg?style=flat-square)](https://www.nuget.org/packages/NetCoreCacheRedis/)
![](https://img.shields.io/badge/license-MIT-green)

## 🔥Core Api And MVC Cache🔥

* Integrate into Redis for API and MVC caching
* Fast, concurrent, evicted memory

## [NetCoreCache( Big MemoryCache )](https://github.com/sj-distributor/core-cache) 👈🏻👈🏻

## 🤟 Install

```
PM     : Install-Package NetCoreCacheRedis
Net CLI: dotnet add package NetCoreCacheRedis
```

## 🚀 Quick start

```C#
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddNetCoreCacheRedis("server=localhost:6379;timeout=5000;MaxMessageSize=1024000;Expire=3600");


// UserController.cs
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [Route("/"), HttpGet]
    [Caching(typeof(Cacheable), "user", "{id}")]
    public User Get([FromQuery] string id)
    {
        return DataUtils.GetData();
    }
}
```

## 📌 Redis cluster

`builder.Services.AddNetCoreCacheRedis("server=127.0.0.1:6000,127.0.0.1:7000,127.0.0.1:6379;db=3;timeout=7000")`

```
This Redis component supports Redis Cluster, configure any node address, 
it will be able to automatically discover other addresses and slot distribution,
and use the correct node when performing read and write operations.

This Redis component does not directly support the Redis sentinel, but supports it in the form of active-standby failover.
```

## Cache automatic eviction

```C#
// UserController.cs
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [Route("/"), HttpGet]
    [Caching(typeof(Cacheable), "user", "{id}", TimeSpan.TicksPerSecond * 2)] // Cache expires after two seconds
    public User Get([FromQuery] string id)
    {
        return DataUtils.GetData();
    }
}

```

## Active cache eviction

```C#
// UserController.cs
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [Route("/"), HttpGet]
    [Caching(typeof(Cacheable), "user", "{id}", TimeSpan.TicksPerHour * 2)] // Cache expires after two hours
    public User Get([FromQuery] string id)
    {
        return DataUtils.GetData();
    }
 
 
    // Requesting this API will trigger the above API cache eviction 👆🏻👆🏻👆🏻
    [Route("/"), HttpPost]
    [Evicting(typeof(CacheEvict), new []{"user"}, "{user:id}")]
    public User Update([FromBody] User user)
    {
        return User;
    }   
}

```

## 👻 Match both uses

```C#
**** ‼️ If the cache is hit, 'Evicting' will only be executed once ‼️ ****

[Route("/evict-and-cache"), HttpGet]
[Caching(typeof(Cacheable), "anson", "QueryId:{id}")]
[Evicting(typeof(CacheEvict), new[] { "anything" }, "QueryId:{id}")]
public IEnumerable<WeatherForecast> Get([FromQuery] string id)
{
    return DataUtils.GetData();
}



**** ‼️ Evicting will always execute ‼️ ****

[Route("/evict-and-cache"), HttpGet]
[Evicting(typeof(CacheEvict), new[] { "anything" }, "QueryId:{id}")]
[Caching(typeof(Cacheable), "anson", "QueryId:{id}")]
public IEnumerable<WeatherForecast> Get([FromQuery] string id)
{
    return DataUtils.GetData();
}
```

## Variable explanation

```
// foo:bar:1 -> "item1"
{
   "foo": {
      "bar": [
         "item1",
         "qux"
     ]
   }
}

// foo:bar:0:url -> "test.weather.com"
{
   "foo": {
      "bar": [
         {
            "url": "test.weather.com",
            "key": "DEV1234567"
         }
     ]
   }
}
```
