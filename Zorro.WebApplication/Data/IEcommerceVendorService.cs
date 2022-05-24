using Zorro.Dal.Models;

namespace Zorro.WebApplication.Data
{
    //ecommerce functions
    public interface IEcommerceVendorService
    {
        string GenerateApiKey(ApplicationUser user);
        string RevokeApiKey(ApplicationUser user, string apikey);
    }

    
}
