using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Middlewares
{
    /// <summary>
    /// A filter that redirects pending merchants to the pending merchants page when attempting to access other pages
    /// </summary>
    public class RedirectPendingMerchant : ActionFilterAttribute
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RedirectPendingMerchant(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // redirect pending merchant to pending merchant page if they're logged in
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var controllerName = descriptor.ControllerName;
            var actionName = descriptor.ActionName;
            if (controllerName != "MerchantController" && actionName != "Pending")
            {
                var userClaims = context.HttpContext.User;
                if (userClaims is not null)
                {
                    var user = _userManager.GetUserAsync(userClaims).GetAwaiter().GetResult();
                    if (user is not null)
                    {
                        var isAdministrator = _userManager.IsInRoleAsync(user, "Administrator").GetAwaiter().GetResult();
                        if (!isAdministrator)
                        {
                            var pendingMerchantFlag = _context.Merchants
                                .Any(m => m.ApplicationUser == user
                           && m.Status == MerchantStatus.Pending);
                            if (pendingMerchantFlag)
                            {
                                var routeValue = new RouteValueDictionary(new { action = "Pending", controller = "Merchant" });
                                context.Result = new RedirectToRouteResult(routeValue);
                            }
                        }
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
