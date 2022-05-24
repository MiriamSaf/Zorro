using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Data;
using Zorro.Dal.Models;
using Zorro.Dal;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers { 

    //shop controller - deals with shop views 
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;

        //DI for shop controller
        public ShopController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IBanker banker)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _banker = banker;
        }

        //show shop home page with the data 
        public ActionResult ShopHome()
        {
            var dashboardData2 = new ShopDashboardViewModel();

            foreach (var shop in _context.Shops)
            {
                dashboardData2.Shops.Add(
                    //shop model  
                    new ShopViewModel()
                    {
                        ShopID = shop.ShopID,
                        ShopName = shop.ShopName,
                        Ordering = shop.Ordering,
                    });
            }

            return View("ShopHome", dashboardData2);
        }

        //checkout page of the shop - returning view 
        public async Task<ActionResult> Checkout(int id)
        {
            TempData["ShopLogin"] = "ShopLogin";
            TempData.Keep();
            var shop = await _context.Shops.FindAsync(id);

            return View("Checkout", shop);
        }

        //shop login page - redirects to sign in page
        public ActionResult ShopLogin()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        //shop confirm page 
        public ActionResult ConfirmPurchase()
        {
            return View("ConfirmPurchase");
        }

        //successful purchase page 
        public async Task<ActionResult> SuccessfulPurchaseAsync()
        {
            //waits for the user, source wallet 
            var user = await _userManager.GetUserAsync(User);

            var sourceWallet = await _banker.GetWalletByDisplayName(user.NormalizedEmail);

            //makes a purchase - demonstration purchase and returns view 
            await _banker.ShopPurchase(sourceWallet, (decimal)40.40);


            return View("SuccessfulPurchase");
        }


    }
}