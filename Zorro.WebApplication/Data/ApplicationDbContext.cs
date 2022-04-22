﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.Dtos;

namespace Zorro.WebApplication.Data
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

    }
}