using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    //home controller pages and views - the base pages of the site
    public class HomeController : Controller
    {
        private readonly IBanker _banker;

        //load in banker with Dependency Injection so can be used in dashboard
        public HomeController(IBanker banker)
        {
            _banker = banker;
        }

        //returns dashboard view
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
                
            return View();
        }

        //loads all that is needed for the dashboard such as the balance of the current user and the 
        //previous transactions to be displayed
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
            //show the view with the data that we have grabbed
            return View("Dashboard", dashboardData);
        }

        //show privacy cshtml page
        public IActionResult Privacy()
        {
            return View();
        }

        //show partner page
        public IActionResult Partner()
        {
            return View();
        }

        //show homeview page
        public IActionResult HomeView()
        {
            return View();
        }

        //show error page if site is not working properly
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}