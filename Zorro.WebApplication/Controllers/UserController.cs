using Microsoft.AspNetCore.Mvc;
using Zorro.WebApplication.Models;

using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Zorro.WebApplication.Models
{
   
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        private IPasswordHasher<ApplicationUser> _passwordHashed;
        public UserController(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHashed)
        {
            _userManager = userManager;
            _passwordHashed = passwordHashed;

        }
        public IActionResult Index()
        {
            //pass all users to view
            return View(_userManager.Users);
        }


        public ViewResult Create() => View();

        //create new user 
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
/*            DateTime baseTime;
            DateTimeOffset sourceTime;
            DateTime targetTime;

            baseTime = new DateTimeOffset((DateTime)user.LockoutEnd, TimeSpan.Zero);*/



            addError(user);
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
                    Email = user.Email,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnd = (DateTime)user.LockoutEnd


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

        public async Task<IActionResult> Update(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        //update user 
        [HttpPost]
        public async Task<IActionResult> Update(string id, string firstname, string surname, string mobile, string email, string password, bool TwoFactorEnabled, DateTime LockoutEnd)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(mobile))
                {
                    user.Mobile = mobile;
                }
                else
                {
                    ModelState.AddModelError("", "mobile field cannot be left empty");
                }
                if (!string.IsNullOrEmpty(firstname))
                {
                    user.FirstName = firstname;
                }
                else
                {
                    ModelState.AddModelError("", "Firstname field cannot be left empty");
                }
                if (!string.IsNullOrEmpty(surname))
                {
                    user.Surname = surname;
                }
                else
                {
                    ModelState.AddModelError("", "Surname field cannot be left empty");
                }
                if (!string.IsNullOrEmpty(email))
                {
                    user.Email = email;
                }
                else
                {
                    ModelState.AddModelError("", "Email field cannot be left empty");
                }

                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = _passwordHashed.HashPassword(user, password);
                }
                else
                {
                    ModelState.AddModelError("", "Password field cannot be empty");
                }

                user.TwoFactorEnabled = TwoFactorEnabled;
                user.LockoutEnd = LockoutEnd;

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(mobile))
                {
                    IdentityResult idResult = await _userManager.UpdateAsync(user);
                    if (idResult.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(idResult);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        //delete 
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        private void addError(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                ModelState.AddModelError("", "password field cannot be left empty");
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("", "username field cannot be left empty");
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                ModelState.AddModelError("", "firstname field cannot be left empty");
            }
            if (string.IsNullOrEmpty(user.Surname))
            {
                ModelState.AddModelError("", "surname field cannot be left empty");
            }
            if (string.IsNullOrEmpty(user.Mobile))
            {
                ModelState.AddModelError("", "mobile field cannot be left empty");
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("", "email field cannot be left empty");
            }


        }
    }
}

