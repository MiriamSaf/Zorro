// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zorro.WebApplication.Models;
using Microsoft.Extensions.Logging;
using Zorro.WebApplication.Data;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
            [Display(Name = "Profile Image")]
            public string AvatarUrl { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [RegularExpression(@"^(\d{6})$", ErrorMessage = "Error: Invalid Expiry Date Entered")]
            [Display(Name = "Credit Card Expiry")]
            public string CCExpiry { get; set; }

            [RegularExpression(@"^(\d{14,16})$", ErrorMessage = "Error: Invalid Credit Card Number")]
            [Display(Name = "Credit Card Number")]
            public string CreditCardNumber { get; set; }

            [DisplayName("Upload File")]
            public IFormFile ImageFile { get; set; }


        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = phoneNumber,
                CreditCardNumber = user.CreditCardNumber,
                CCExpiry = user.CCExpiry,
                ImageFile = user.ImageFile
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync(InputModel input, IFormFile formFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);

                user.CreditCardNumber = input.CreditCardNumber;
                user.CCExpiry = input.CCExpiry;
                user.AvatarUrl = input.AvatarUrl;
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

            // Damien Attempt

/*            Debug.WriteLine(input.ImageFile.FileName);
            Debug.WriteLine(Input.ImageFile.FileName);
            Debug.WriteLine(Input.ImageFile.Name);
            Debug.WriteLine(input.ImageFile.Name);

            user.AvatarUrl = Input.ImageFile.FileName;

            Debug.WriteLine(user.AvatarUrl);

            string wwwRootPath = _hostingEnv.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(input.ImageFile.FileName);
            string extension = Path.GetExtension(input.ImageFile.FileName);
            user.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/images/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await input.ImageFile.CopyToAsync(fileStream);
            }*/


            // Miriam Attempt

            /*            var filename = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", formFile.FileName);
                        using (System.IO.Stream stream = new FileStream(path, FileMode.Create))
                            await formFile.CopyToAsync(stream);*/

            // model.ImageFile = filename;
            //_context.Add(model);
            //_context.SaveChanges();


            user.CreditCardNumber = Input.CreditCardNumber;
            user.CCExpiry = Input.CCExpiry;
            user.AvatarUrl = Input.AvatarUrl;
            await _userManager.UpdateAsync(user);
            var result = await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
