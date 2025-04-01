using System.Diagnostics;
using GestorAlquilerVehiculos.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestorAlquilerVehiculos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("~/Views/Pages/Index.cshtml");
        }

        public IActionResult About()
        {
            return View("~/Views/Pages/About.cshtml");
        }

        public IActionResult Blog()
        {
            return View("~/Views/Pages/Blog.cshtml");
        }

        public IActionResult BlogSingle()
        {
            return View("~/Views/Pages/BlogSingle.cshtml");
        }

        public IActionResult Car()
        {
            return View("~/Views/Pages/Car.cshtml");
        }

        public IActionResult CarSingle()
        {
            return View("~/Views/Pages/CarSingle.cshtml");
        }

        public IActionResult Contact()
        {
            return View("~/Views/Pages/Contact.cshtml");
        }

        public IActionResult Main()
        {
            return View("~/Views/Pages/Main.cshtml");
        }

        public IActionResult Pricing()
        {
            return View("~/Views/Pages/Pricing.cshtml");
        }

        public IActionResult Services()
        {
            return View("~/Views/Pages/Services.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AdminPanel()
        {
            return View("~/Views/Pages/AdminPanel.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}