using ApiForTest.Utils;
using Core.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ApiForTest.Controllers;

public class HomeController : Controller
{
    
    [Caching(typeof(Cacheable), "page", "view:{id}", TimeSpan.TicksPerSecond * 2)]
    public IActionResult Index(string id)
    {
        return View(DataUtils.GetData());
    }

    [Evicting(typeof(CacheEvict), new []{"page"}, "view:{id}")]
    public IActionResult Tow(string id)
    {
        return View(DataUtils.GetData());
    }
}