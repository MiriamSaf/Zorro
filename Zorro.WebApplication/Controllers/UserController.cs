using Microsoft.AspNetCore.Mvc;
using Zorro.Dal.Models;

using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Zorro.WebApplication.ViewModels;
using Zorro.Dal;

namespace Zorro.WebApplication.Models
{
   
    public class UserController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHashed;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHashed,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _passwordHashed = passwordHashed;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            // build view items
            var viewModel = new UserAdminViewModel();

            // load users
            foreach (var user in _userManager.Users.ToList())
            {
                var userViewModel = await MapUserToViewModel(user);
                viewModel.Users.Add(userViewModel);
            }

            // load merchants
            viewModel.Merchants = _applicationDbContext.Merchants.ToList();

            return View(viewModel);
        }

        private async Task<UserViewModel> MapUserToViewModel(ApplicationUser user)
        {
            var userViewItem = new UserViewModel()
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                LockedOut = false,
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            if (await _userManager.IsInRoleAsync(user, "Administrator"))
                userViewItem.AdminAccessGranted = true;

            // show as merchant or consumer
            if (await _userManager.IsInRoleAsync(user, "Merchant"))
                userViewItem.Role = "Merchant";
            else
                userViewItem.Role = "Consumer";

            return userViewItem;
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
            var user = await _userManager.FindByIdAsync(id);
            if (user is null )
                return RedirectToAction("Index");

            var userViewModel = await MapUserToViewModel(user);
            ViewBag.Roles = new string[] { "Merchant", "Consumer" };
            return View(userViewModel);
        }

        //update user 
        [HttpPost]
        public async Task<IActionResult> Update(UserViewModel userViewModel)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.Id);
            if (user != null)

            ModelState.AddModelError("", "User Not Found");
            // Add and remove roles

            if (userViewModel.Role == "Merchant")
            {
                await _userManager.AddToRoleAsync(user, "Merchant");
                await _userManager.RemoveFromRoleAsync(user, "Consumer");
            }

            if (userViewModel.Role == "Consumer")
            {
                await _userManager.AddToRoleAsync(user, "Consumer");
                await _userManager.RemoveFromRoleAsync(user, "Merchant");
            }

            if (userViewModel.AdminAccessGranted)
                await _userManager.AddToRoleAsync(user, "Administrator");
            else
                await _userManager.RemoveFromRoleAsync(user, "Administrator");

            if (!string.IsNullOrEmpty(userViewModel.Mobile))
            {
                user.Mobile = userViewModel.Mobile;
            }
            else
            {
                ModelState.AddModelError("", "mobile field cannot be left empty");
            }
            if (!string.IsNullOrEmpty(userViewModel.FirstName))
            {
                user.FirstName = userViewModel.FirstName;
            }
            else
            {
                ModelState.AddModelError("", "Firstname field cannot be left empty");
            }
            if (!string.IsNullOrEmpty(userViewModel.Surname))
            {
                user.Surname = userViewModel.Surname;
            }
            else
            {
                ModelState.AddModelError("", "Surname field cannot be left empty");
            }
            if (!string.IsNullOrEmpty(userViewModel.Email))
            {
                user.Email = userViewModel.Email;
            }
            else
            {
                ModelState.AddModelError("", "Email field cannot be left empty");
            }

            if (!string.IsNullOrEmpty(null))
            {
                user.PasswordHash = _passwordHashed.HashPassword(user, null);
            }

            IdentityResult idResult = await _userManager.UpdateAsync(user);
            if (idResult.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(idResult);

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

