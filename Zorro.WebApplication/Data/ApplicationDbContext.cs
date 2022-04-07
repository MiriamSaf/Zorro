using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {}

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<BpayBiller> Payees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}