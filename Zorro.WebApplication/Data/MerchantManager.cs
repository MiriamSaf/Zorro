using Microsoft.AspNetCore.Identity;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Data
{
    public class MerchantManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Banker> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MerchantManager(UserManager<ApplicationUser> userManager, ILogger<Banker> logger, ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
        }

        public async Task MigrateConsumerToMerchant(string userId, string businessName, string abn)
        {
            const string merchantRoleName = "Merchant";
            // find user and check if they are already a merchant
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new Exception("Unable to find user");
            if (await _userManager.IsInRoleAsync(user, "Merchant"))
                throw new Exception("User is already a merchant");
            
            // assign role and create merchant
            var result = await _userManager.AddToRoleAsync(user, merchantRoleName);
            if (!result.Succeeded)
                throw new Exception($"Unable to assign user the {merchantRoleName}");

            var merchant = new Merchant()
            {
                Abn = abn,
                ApplicationUser = user,
                BusinessName = businessName,
            };
            await _applicationDbContext.Merchants.AddAsync(merchant);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
