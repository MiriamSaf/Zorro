using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Zorro.Dal;
using Zorro.Dal.Models;
using Zorro.WebApplication.Data;
using Zorro.WebApplication.ViewModels;

namespace Zorro.WebApplication.Models
{
    //merchant controller - deals with displaying merchant views, pages and detail
    [Authorize]
    public class MerchantController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHashed;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IBanker _banker;

        //DI for merchant controller
        public MerchantController(UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHashed, ApplicationDbContext applicationDbContext, IBanker banker)
        {
            _userManager = userManager;
            _passwordHashed = passwordHashed;
            _applicationDbContext = applicationDbContext;
            _banker = banker;
        }

        //return merchant index - default page
        public async Task<IActionResult> Index()
        {
            // validate wallet and show balance
            var wallet = await _banker.GetWalletByDisplayName(User.Identity.Name);
            var apiKey = string.IsNullOrEmpty(wallet.ApplicationUser.APIKey) == true ? "" : wallet.ApplicationUser.APIKey;
            var dashboardData = new MerchantDashboardViewModel()
            {
                Balance = wallet.Balance,
                WalletId = wallet.DisplayName,
                ApiKey = apiKey
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
            return View("Index", dashboardData);
        }

        //update Api key view behind the scenes 
        public async Task<IActionResult> UpdateAPIKey()
        {
            //pass all users to view
            var user = await _userManager.GetUserAsync(User);

            //set the new key by a random generator
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);

            user.APIKey = apiKey;

            await _userManager.UpdateAsync(user);
            //send the view with the updates user and their new key
            return RedirectToAction("Index");
        }

        //return register view for merchant 
        public ViewResult Register() => View();
        
        //register post view for merchants 
        [HttpPost]
        public async Task<IActionResult> Register(Merchant merchant)
        {
            var user = await _userManager.GetUserAsync(User);
            var foundMerchant = _applicationDbContext.Merchants.Where(m => m.ApplicationUser == user && m.Status == MerchantStatus.Approved);
            //ensure the user is not already a merchant
            if (foundMerchant.Any())
                throw new Exception("User is already related to an active merchant");
            merchant.ApplicationUser = user;
            merchant.Status = MerchantStatus.Pending;
            //add the merchant to the database
            await _applicationDbContext.Merchants.AddAsync(merchant);
            await _applicationDbContext.SaveChangesAsync();
            //return the pending view
            return View("Pending");
        }

        // Pending merchant page
        public IActionResult Pending() => View("Pending");

        //approved merchant view
        public async Task<IActionResult> Approve(int id)
        {
            var merchant = await _applicationDbContext.Merchants.FindAsync(id);
            if (merchant is null)
                throw new Exception($"Unable to find merchant with id {id}");
            merchant.Status = MerchantStatus.Approved;
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        }

        //create merchant view 
        public ViewResult Create() => View();

        //create new user 
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            addError(user);
            if (ModelState.IsValid)
            {
                //sets the new user to the passed in fields
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
                };

               
                //create the new user
                IdentityResult result = await _userManager.CreateAsync(appUser, user.PasswordHash);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    //it didn't go through so add an error
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        //update user by passing in their unique id
        public async Task<IActionResult> Update(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        //update user post method to save in the DB
        [HttpPost]
        public async Task<IActionResult> Update(string id, string firstname, string surname, string mobile, string email, string password, bool TwoFactorEnabled, DateTime LockoutEnd)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            //check that fields required are all not empty
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
                //if all is filled out and not empty fields
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(mobile))
                {
                    //update the user and return to index page
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

        //delete user by passing in their id
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            //check user exists that are trying to delete
            if (user != null)
            {
                //delete the user
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

        // delete a merchant by their ID
        [HttpPost]
        public async Task<IActionResult> DeleteMerchant(int id)
        {
            var foundMerchant = await _applicationDbContext.Merchants.FindAsync(id);
            _applicationDbContext.Merchants.Remove(foundMerchant);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index","User");
        }

        //errors function to display error
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        //add error to model state if empty 
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

