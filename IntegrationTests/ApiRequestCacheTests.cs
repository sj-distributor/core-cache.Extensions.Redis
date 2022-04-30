using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiForTest.Entity;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests;

[Collection("Sequential")]

public class ApiRequestCacheTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    public ApiRequestCacheTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async void RequestCanCache()
    {
        
        var resp1 = await _httpClient.GetAsync("/?id=1");
        var result1 = await resp1.Content.ReadAsStringAsync();

        var resp2 = await _httpClient.GetAsync("/?id=1");
        var result2 = await resp2.Content.ReadAsStringAsync();

        Assert.Equal(result1, result2);
    }

    [Fact]
    public async void RequestCanCacheAndWillTimeout()
    {
        var resp1 = await _httpClient.GetAsync("/?id=2");
        var result1 = await resp1.Content.ReadAsStringAsync();

        await Task.Delay(TimeSpan.FromSeconds(3));
        
        var resp2 = await _httpClient.GetAsync("/?id=2");
        var result2 = await resp2.Content.ReadAsStringAsync();

        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public async void CacheCanEvict()
    {
        var resp1 = await _httpClient.GetAsync("/?id=3");
        var result1 = await resp1.Content.ReadAsStringAsync();

        await _httpClient.PostAsync("/?id=3", null);

        var resp2 = await _httpClient.GetAsync("/?id=3");
        var result2 = await resp2.Content.ReadAsStringAsync();

        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public async void RequestCanParserBodyParameterAndWillCache()
    {
        var users = new List<User>()
        {
            new()
            {
                Id = "1",
                Name = "anson",
            },
            new()
            {
                Id = "2",
                Name = "anson2",
            },
        };
        var resp1 = await _httpClient.PostAsJsonAsync("/users", users);
        var result1 = await resp1.Content.ReadAsStringAsync();

        var resp2 = await _httpClient.PostAsJsonAsync("/users", users);
        var result2 = await resp2.Content.ReadAsStringAsync();

        Assert.Equal(result1, result2);
    }
    
    [Fact]
    public async void CacheAndEvictOther()
    {
        var resp1 = await _httpClient.GetAsync("/?id=4");
        var result1 = await resp1.Content.ReadAsStringAsync();

        var resp2 = await _httpClient.GetAsync("/evict-and-cache?id=4");
        var result2 = await resp2.Content.ReadAsStringAsync();

        var resp3 = await _httpClient.GetAsync("/?id=4");
        var result3 = await resp3.Content.ReadAsStringAsync();
        
        var resp4 = await _httpClient.GetAsync("/evict-and-cache?id=4");
        var result4 = await resp4.Content.ReadAsStringAsync();

        Assert.NotEqual(result1, result3);
        Assert.Equal(result2, result4);
    }
    
    [Fact]
    public async void CanTriggerEmptyNameOrKeyRequest()
    {
        var resp1 = await _httpClient.GetAsync("/users?id=5");
        var result1 = await resp1.Content.ReadAsStringAsync();

        var resp2 = await _httpClient.GetAsync("/users?id=5");
        var result2 = await resp2.Content.ReadAsStringAsync();

        Assert.NotEqual(result1, result2);
    }
}