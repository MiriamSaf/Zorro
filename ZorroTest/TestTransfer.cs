using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zorro.Dal;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;
using Zorro.WebApplication.Data;
using System.Threading.Tasks;

namespace ZorroTest
{
    
    //testing transfer method - all scenarios 
    [TestClass]
    public class TestTransfer
    {
        //test transfer with less than zero transfer amount
        [ExpectedException(typeof(InvalidTransferAmountException))]
        [TestMethod]
        public async Task TestTransferWithLessThanZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Tester" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to transfer 0 
            await bankerService.TransferFunds(wallet1, wallet2, -7M, "my transfer less than zero");
        }

        //test transfer with less than zero transfer amount
        [ExpectedException(typeof(InvalidTransferAmountException))]
        [TestMethod]
        public async Task TestTransferWithZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Tester" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to transfer 0 
            await bankerService.TransferFunds(wallet1, wallet2, 0M, "my transfer zero dollars");
        }

        //test transfer with sufficient funds so should go through
        [TestMethod]
        public async Task TransferWithSufficientFunds()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDB")
            .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Smith" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 10.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            await bankerService.TransferFunds(wallet1, wallet2, 2.00M, "transfer allowed as have funds");
        }


        //test transfer with insufficient funds in recipient wallet
        [ExpectedException(typeof(InsufficientFundsException))]
        [TestMethod]
        public async Task TransferWithInsufficientFunds()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDB")
            .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Smith" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 10.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            await bankerService.TransferFunds(wallet1, wallet2, 11.00M, "transfer more than have in wallets");
        }

        //test refund positive amount
        [ExpectedException(typeof(InvalidTransferAmountException))]
        [TestMethod]
        public async Task TransferWithRefund()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDB")
            .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Smith" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 10.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            await bankerService.TransferFunds(wallet1, wallet2, 5.00M, "refund positive", Currency.Aud, TransactionType.Refund);
        }


        //test transfer with greater than 2 decimal places amount
        [ExpectedException(typeof(InvalidTransferAmountException))]
        [TestMethod]
        public async Task TransferWithMoreThan2Dec()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDB")
            .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Smith" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 10.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            await bankerService.TransferFunds(wallet1, wallet2, 5.123M, "decimal 3", Currency.Aud, TransactionType.Transfer);
        }


        //tranfer negative refund amount - following rules so should pass
        [TestMethod]
        public async Task TransferWithRefundNegative()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "FakeDB")
            .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Smith" };
            var user2 = new ApplicationUser() { FirstName = "Keith", Surname = "Bazza" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 10.00M, ApplicationUser = user1 };
            var wallet2 = new Wallet() { Balance = 10.00M, ApplicationUser = user2 };
            var wallets = new Wallet[] { wallet1, wallet2 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            await bankerService.TransferFunds(wallet1, wallet2, -5.00M, "refund negative-should work", Currency.Aud, TransactionType.Refund);
        }

    }
}
