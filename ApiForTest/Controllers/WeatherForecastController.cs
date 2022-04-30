using ApiForTest.Entity;
using ApiForTest.Utils;
using Core.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ApiForTest.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [Route("/"), HttpGet]
    [Caching(typeof(Cacheable), "anything", "QueryId:{id}", TimeSpan.TicksPerSecond * 2)]
    public IEnumerable<WeatherForecast> Get([FromQuery] string id)
    {
        return DataUtils.GetData();
    }

    [Route("/evict-and-cache"), HttpGet]
    [Evicting(typeof(CacheEvict), new[] { "anything" }, "QueryId:{id}")]
    [Caching(typeof(Cacheable), "anson", "QueryId:{id}")]
    public IEnumerable<WeatherForecast> Get2([FromQuery] string id)
    {
        return DataUtils.GetData();
    }
    
    [Route("/pattern"), HttpPost]
    [Evicting(typeof(CacheEvict), new[] { "anything" }, "QueryId:{id}")]
    public IEnumerable<WeatherForecast> PostPattern([FromQuery] string id)
    {
        return DataUtils.GetData();
    }

    [Route("/"), HttpPost]
    [Evicting(typeof(CacheEvict), new[] { "anything" }, "QueryId:{id}")]
    public IEnumerable<WeatherForecast> Post([FromQuery] string id)
    {
        return DataUtils.GetData();
    }

    [Route("/users"), HttpPost]
    [Caching(typeof(Cacheable), "post", "{users:1:id}")]
    public IEnumerable<WeatherForecast> PostUsers([FromBody] List<User> users)
    {
        return DataUtils.GetData();
    }

    [Route("/users"), HttpGet]
    [Caching(typeof(Cacheable), "", "")]
    public IEnumerable<WeatherForecast> GetUsers([FromQuery] string id)
    {
        return DataUtils.GetData();
    }
}