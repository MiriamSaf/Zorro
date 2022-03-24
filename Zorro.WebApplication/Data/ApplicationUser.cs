using Microsoft.AspNetCore.Identity;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public string? AvatarUrl { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
