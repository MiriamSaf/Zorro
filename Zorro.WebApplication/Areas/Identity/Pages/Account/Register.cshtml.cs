// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = applicationDbContext;
        }

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Required, System.ComponentModel.DisplayName("First Name")]
            [StringLength(50, ErrorMessage = "Error: First Name has a maximum of 50 Characters.")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(60, ErrorMessage = "Error: Surname has a maximum of 60 Characters.")]
            public string Surname { get; set; }


            [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
            public string Mobile { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; }

        }

        // code to get users age diff from today and birthdate adapted from
        //https://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-based-on-a-datetime-type-birthday
        public static int GetAge(DateTime birthDate)
        {
            DateTime today = DateTime.Now; // To avoid a race condition around midnight
            int age = today.Year - birthDate.Year;

            if (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day))
                age--;

            return age;
        }




        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");
           
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

           //if user tries for future
            if (Input.BirthDate >= DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "You must be at least 18+ Years to Access this Service");
                return Page();
            }

            var dateOBirth = Input.BirthDate;
            var ageReturn  = GetAge((DateTime)dateOBirth);

            if(ageReturn <= 18)
            {
                ModelState.AddModelError(string.Empty, "You must be at least 18+ Years to Access this Service");
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                var key = new byte[32];
                using (var generator = RandomNumberGenerator.Create())
                    generator.GetBytes(key);
                string apiKey = Convert.ToBase64String(key);

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FirstName = Input.FirstName;
                user.Surname = Input.Surname;
                user.BirthDate = Input.BirthDate;
                user.Mobile = Input.Mobile;
                user.Avatar = "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";
                user.APIKey = apiKey;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await CreateAccount(user);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task CreateAccount(ApplicationUser user)
        {
            // Create default wallet for new registered user
            Wallet wallet = new()
            {
                ApplicationUser = user
            };
            _context.Add(wallet);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created account for new user with ID {accountId}", user.NormalizedEmail);
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
