using System.Security.Cryptography;

using Zorro.Dal.Models;

namespace Zorro.WebApplication.Data
{
    public interface IEcommerceVendorService
    {
        string GenerateApiKey(ApplicationUser user);
        string RevokeApiKey(ApplicationUser user, string apikey);
    }

    
}
