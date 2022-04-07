using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
                
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var dashboardData = new DashboardViewModel()
            {
                Balance = 55.42M,
                WalletId = User.Identity.Name
            };
            dashboardData.RecentTransactions.Add(
            new TransactionViewModel()
            {
                Amount = 21.32M,
                Date = DateTime.Now,
                Description = "Maccas"
            });
            return View("Dashboard", dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult HomeView()
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