using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Data;
using Zorro.Dal.Models;
using Zorro.Dal;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;


        public ShopController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IBanker banker)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _banker = banker;
        }

        public ActionResult ShopHome()
        {
            var dashboardData2 = new ShopDashboardViewModel();

            foreach (var shop in _context.Shops)
            {
                dashboardData2.Shops.Add(
                    new ShopViewModel()
                    {
                        ShopID = shop.ShopID,
                        ShopName = shop.ShopName,
                        Ordering = shop.Ordering,
                    });
            }

            return View("ShopHome", dashboardData2);
        }

        public async Task<ActionResult> Checkout(int id)
        {
            TempData["ShopLogin"] = "ShopLogin";
            TempData.Keep();
            var shop = await _context.Shops.FindAsync(id);

            return View("Checkout", shop);
        }

        public ActionResult ShopLogin()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public ActionResult ConfirmPurchase()
        {
            return View("ConfirmPurchase");
        }

        public async Task<ActionResult> SuccessfulPurchaseAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);

            await _banker.ShopPurchase(sourceWallet, (decimal)40.40);


            return View("SuccessfulPurchase");
        }


    }
}