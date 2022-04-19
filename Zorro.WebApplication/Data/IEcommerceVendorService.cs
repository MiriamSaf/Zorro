using System.Security.Cryptography;

namespace Zorro.WebApplication.Data
{
    public interface IEcommerceVendorService
    {
        string GenerateApiKey(ApplicationUser user);
        string RevokeApiKey(ApplicationUser user, string apikey);
    }

    
}
