// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Zorro.Dal.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Zorro.WebApplication.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IHostingEnvironment _hostingEnv;

        [Obsolete]
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnv = hostingEnv;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [RegularExpression(@"^(([1-9][0-2]?)|([0-0][1-9]))/([2-9][0-9]{1})$", ErrorMessage = "Error: Invalid Expiry Date Entered.Number must be 4 Digits with a / in between numbers - as in MM/YY")]
            [Display(Name = "Credit Card Expiry")]
            public string CCExpiry { get; set; }

            [RegularExpression(@"^(\d{14,16})$", ErrorMessage = "Error: Invalid Credit Card Number. Enter digits that are not separated by hyphen or comma")]
            [Display(Name = "Credit Card Number")]
            public string CreditCardNumber { get; set; }
            public IFormFile Avatar { get; set; }

            public string FirstName { get; set; }

            public string Surname { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; }

            public string Mobile { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                CreditCardNumber = user.CreditCardNumber,
                CCExpiry = user.CCExpiry,
                FirstName = user.FirstName,
                Surname = user.Surname,
                BirthDate = user.BirthDate,
                Mobile = user.Mobile
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public InputModel GetInput()
        {
            return Input;
        }

        public async Task<IActionResult> OnPostAsync(InputModel input, IFormFile avatar_file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }



            if (!ModelState.IsValid)
            {
                await LoadAsync(user);

                user.FirstName = Input.FirstName;
                user.Surname = Input.Surname;
                user.BirthDate = Input.BirthDate;
                user.Mobile = user.Mobile;
                user.CreditCardNumber = input.CreditCardNumber;
                user.CCExpiry = input.CCExpiry;


                await _userManager.UpdateAsync(user);

                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (avatar_file != null)
            {
                var ms = new MemoryStream();
                await avatar_file.CopyToAsync(ms);
                string base64String = Convert.ToBase64String(ms.ToArray());

                user.Avatar = base64String;
            }

            user.FirstName = Input.FirstName;
            user.Surname = Input.Surname;
            user.BirthDate = Input.BirthDate;
            user.Mobile = user.Mobile;

            user.CreditCardNumber = Input.CreditCardNumber;
            user.CCExpiry = Input.CCExpiry;
            await _userManager.UpdateAsync(user);
            var result = await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
