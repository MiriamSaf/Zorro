using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.Dtos;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBanker _banker;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopController(ILogger<ShopController> logger, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IBanker banker)
        {
            _logger = logger;
            _context = applicationDbContext;
            _userManager = userManager;
            _banker = banker;
        }

        public ActionResult ShopHome()
        {
            return View("ShopHome");
        }

        public ActionResult Checkout()
        {
            return View("Checkout");
        }

        public ActionResult ShopLogin()
        {
            return View("ShopLogin");
        }

        public ActionResult ConfirmPurchase()
        {
            return View("ConfirmPurchase");
        }

        public ActionResult SuccessfulPurchase()
        {
            return View("SuccessfulPurchase");
        }


    }
}