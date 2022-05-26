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

    //test deposit scenarios
    [TestClass]
    public class DepositTest
    {

        //test deposit with zero as amount
        [ExpectedException(typeof(InvalidDepositAmountException))]
        [TestMethod]
        public async Task TestDepositWithZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser()
            {
                FirstName = "John",
                Surname = "Test",
                CreditCardNumber = "1111222233334444",
                CCExpiry = "11/23"
            };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };

            await context.Wallets.AddAsync(wallet1);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to depsoit 0 
            await bankerService.DepositFunds(wallet1, 0, "deposit zero");
        }

        //test deposit with negative number as amount
        [ExpectedException(typeof(InvalidDepositAmountException))]
        [TestMethod]
        public async Task TestDepositWithNegative()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser()
            {
                FirstName = "John",
                Surname = "Test",
                CreditCardNumber = "1111222233334444",
                CCExpiry = "11/23"
            };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };

            await context.Wallets.AddAsync(wallet1);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to deposit negative
            await bankerService.DepositFunds(wallet1, -19, "deposit negative");
        }

        //test deposit with number with >=3 decimal in amount
        [ExpectedException(typeof(InvalidDepositAmountException))]
        [TestMethod]
        public async Task TestDepositWithGreaterDecimal()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser()
            {
                FirstName = "John",
                Surname = "Test",
                CreditCardNumber = "1111222233334444",
                CCExpiry = "11/23"
            };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };

            await context.Wallets.AddAsync(wallet1);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to deposit negative
            await bankerService.DepositFunds(wallet1, 19.2345M, "deposit great decimal amount");
        }

        //test deposit with valid posotive number as amount
        [TestMethod]
        public async Task TestDepositWithPositive()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser()
            {
                FirstName = "John",
                Surname = "Test",
                CreditCardNumber = "1111222233334444",
                CCExpiry = "11/23"
            };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };

            await context.Wallets.AddAsync(wallet1);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to depsoit 0 
            await bankerService.DepositFunds(wallet1, 19, "deposit allowed");
            Assert.AreEqual(39.00M, wallet1.Balance);
        }
    }
}