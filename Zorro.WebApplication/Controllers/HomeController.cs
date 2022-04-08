using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _applicationDBContext;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDBContext, IBanker banker, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _applicationDBContext = applicationDBContext;
            _banker = banker;
            _userManager = userManager;
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
        public async Task<IActionResult> Dashboard()
        {
            // validate wallet and show balance
            var wallet = await _banker.GetWalletByDisplayName(User.Identity.Name);
            var dashboardData = new DashboardViewModel()
            {
                Balance = wallet.Balance,
                WalletId = wallet.DisplayName
            };

            // get transactions and return transactions view models
            var transactions = await _banker.GetTransactionsByWallet(wallet.Id);
            foreach (var transaction in transactions.OrderByDescending(t => t.TransactionTimeUtc))
            {
                dashboardData.RecentTransactions.Add(
                    new TransactionViewModel()
                    {
                        Amount = transaction.Amount,
                        Date = transaction.TransactionTimeUtc,
                        Description = transaction.Comment,
                    });
            }
            
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