using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {}

        public DbSet<Account> Acconuts { get; set; }
        public DbSet<BillPay> BillPay { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}