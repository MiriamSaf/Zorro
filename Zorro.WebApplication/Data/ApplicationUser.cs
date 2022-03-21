using Microsoft.AspNetCore.Identity;

namespace Zorro.WebApplication.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AvatarUrl { get; set; }
    }
}
