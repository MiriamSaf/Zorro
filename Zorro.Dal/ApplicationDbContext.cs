using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;

namespace Zorro.Dal
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // composite key for remembered billers
            modelBuilder.Entity<RememberedBiller>()
                .HasKey(b => new { b.BillerCode, b.ApplicationUserId });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<BpayBiller> Payees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RememberedBiller> RememberedBillers { get; set; }
        public DbSet<Shops> Shops { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<MerchantApiKey> ApiKeys { get; set; }

    }
}