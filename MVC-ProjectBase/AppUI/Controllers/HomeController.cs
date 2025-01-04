using AutoMapper;
using Domain;
using Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace AppUI.Controllers
{
    public class HomeController : CustomController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger,
                              IMemoryCache cache,
                              IMapper mapper,
                              ApplicationUtils utils)
            : base(logger, cache, mapper, utils)
        {
        }

        public IActionResult Index()
        {
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
}
