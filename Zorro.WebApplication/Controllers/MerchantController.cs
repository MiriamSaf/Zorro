﻿using Microsoft.AspNetCore.Mvc;
using Zorro.Dal.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Zorro.Dal;

namespace Zorro.WebApplication.Models
{
   
    public class MerchantController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHashed;
        private readonly ApplicationDbContext _applicationDbContext;

        public MerchantController(UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHashed, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _passwordHashed = passwordHashed;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            //pass all users to view
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        public async Task<IActionResult> UpdateAPIKey()
        {
            //pass all users to view
            var user = await _userManager.GetUserAsync(User);

            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);

            user.APIKey = apiKey;

            await _userManager.UpdateAsync(user);

            return View("Index",user);
        }

        public ViewResult Register() => View();
        
        [HttpPost]
        public async Task<IActionResult> Register(Merchant merchant)
        {
            var user = await _userManager.GetUserAsync(User);
            var foundMerchant = _applicationDbContext.Merchants.Where(m => m.ApplicationUser == user && m.Status == MerchantStatus.Approved);
            if (foundMerchant.Any())
                throw new Exception("User is already related to a active merchant");
            merchant.ApplicationUser = user;
            merchant.Status = MerchantStatus.Pending;
            await _applicationDbContext.Merchants.AddAsync(merchant);
            await _applicationDbContext.SaveChangesAsync();
            return View("Pending");
        }

        public async Task<IActionResult> Approve(int id)
        {
            var merchant = await _applicationDbContext.Merchants.FindAsync(id);
            if (merchant is null)
                throw new Exception($"Unable to find merchant with id {id}");
            merchant.Status = MerchantStatus.Approved;
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        }

        public ViewResult Create() => View();

        //create new user 
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
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
                    //LockoutEnd = (DateTime)user.LockoutEnd
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

