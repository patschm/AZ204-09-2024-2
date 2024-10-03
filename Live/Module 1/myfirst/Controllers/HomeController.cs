using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myfirst.Models;

namespace myfirst.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration config)
    {
        _logger = logger;
        _configuration = config;
    }

    public IActionResult Index()
    {
        ViewBag.Env = Environment.GetEnvironmentVariable("ENV_TEST");
        ViewBag.Set = _configuration.GetSection("MyFirst:Name").Value;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
