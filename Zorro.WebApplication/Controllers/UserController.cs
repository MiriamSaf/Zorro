using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Models;

using Microsoft.AspNetCore.Identity;

namespace Zorro.WebApplication.Models
{
   
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //pass all users to view
            return View(_userManager.Users);
        }


        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    BirthDate = user.BirthDate,
                    PhoneNumber = user.PhoneNumber,
                    Mobile = user.Mobile,
                    PasswordHash = user.PasswordHash,
                    Email = user.Email

                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.PasswordHash);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
    }
}

