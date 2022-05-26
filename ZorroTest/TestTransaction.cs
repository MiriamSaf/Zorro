using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zorro.WebApplication;
using Zorro.WebApplication.Controllers;
using Zorro.WebApplication.Models;
using Zorro.WebApplication.ViewModels;
using Zorro.WebApplication.Data;
using Zorro.Dal.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal;

namespace ZorroTest
{
    [TestClass]
    public class TestTransaction
    {

        [TestMethod]
        public async Task TestGetWallet()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test", CreditCardNumber = "1111222233334444", CCExpiry = "11/23" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to get wallet
            await bankerService.GetTransactionsByWallet(walletId: wallet1.Id);
               
        }

        //transfer to fake db
        [TestMethod]
        public async Task TestTransferTrans()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test", CreditCardNumber = "1111222233334444", CCExpiry = "11/23" };

            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 300M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2};

            //create TRANSF trans
            var sourceTransaction = new Transaction()
            {
                Amount = 6.00M * -1,
                Comment = "transf",
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = DateTime.Now,
                TransactionType = TransactionType.Payment,
                Wallet = wallet1

            };

            var destTransaction = new Transaction()
            {
                Amount = 6.00M,
                Comment = "transf",
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = DateTime.Now,
                TransactionType = TransactionType.Payment,
                Wallet = wallet2

            };

            await context.AddAsync(sourceTransaction);
            wallet1.Balance += sourceTransaction.Amount;

            await context.AddAsync(destTransaction);
            wallet2.Balance += destTransaction.Amount;

            await context.SaveChangesAsync();

        }

        //deposit transaction to DB
        [TestMethod]
        public async Task TestCreateDeposTrans()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test", CreditCardNumber = "1111222233334444", CCExpiry = "11/23" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            //create bpay trans
            var depTransaction = new Transaction()
            {
                Amount = 600.00M,
                Comment = "dep",
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = DateTime.Now,
                TransactionType = TransactionType.Payment,
                Wallet = wallet1

            };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            await context.AddAsync(depTransaction);
            context.SaveChanges();

        }

        [TestMethod]
        public async Task TestCreateBpayTrans()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test", CreditCardNumber = "1111222233334444", CCExpiry = "11/23" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            //create bpay trans
            var bpayTransaction = new Transaction()
            {
                Amount = 2.00M,
                Comment = "billpay",
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = DateTime.Now,
                TransactionType = TransactionType.BPay,
                Wallet = wallet1

            };


            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            await context.AddAsync(bpayTransaction);
            context.SaveChanges();

        }

    }
}
