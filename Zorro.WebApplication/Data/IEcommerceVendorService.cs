using System.Security.Cryptography;

using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public interface IEcommerceVendorService
    {
        string GenerateApiKey(ApplicationUser user);
        string RevokeApiKey(ApplicationUser user, string apikey);
    }

    
}
