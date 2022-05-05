using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zorro.Dal;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;
using Zorro.WebApplication.Data;
using System.Threading.Tasks;

namespace ZorroTest
{
    //testing billpay method - all scenarios 
    [TestClass]
    public class TestBillPay
    {
        
            //test billpay with less than zero transfer amount
            [ExpectedException(typeof(InvalidBillPayAmountException))]
            [TestMethod]
            public async Task TestBillPayWithLessThanZero()
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "FakeDB")
               .Options;

                var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };

                // Insert seed data into the database using one instance of the context
                using var context = new ApplicationDbContext(options);
                var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
                var wallets = new Wallet[] { wallet1 };

                await context.Wallets.AddRangeAsync(wallets);
                context.SaveChanges();

                var bankerService = new BankerService(context);
                //try to transfer 0 
                await bankerService.BpayTransfer(wallet1,  -7M, 1, "my billpay", Currency.Aud, TransactionType.BPay);
            }

        //test billpay with zero transfer amount
        [ExpectedException(typeof(InvalidBillPayAmountException))]
        [TestMethod]
        public async Task TestBillPayWithZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to transfer 0 
            await bankerService.BpayTransfer(wallet1, 0M, 1, "my billpay less than zero", Currency.Aud, TransactionType.BPay);
        }

        //test billpay with more than exists in wallet
        [ExpectedException(typeof(InvalidBillPayAmountException))]
        [TestMethod]
        public async Task TestBillPayWithGreaterThanFunds()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to transfer 0 
            await bankerService.BpayTransfer(wallet1, 30M, 1, "my billpay more than have in wallet", Currency.Aud, TransactionType.BPay);
        }

        //test billpay with allowed +ve transfer amount
        [TestMethod]
        public async Task TestBillPayWithPosotive()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "FakeDB")
           .Options;

            var user1 = new ApplicationUser() { FirstName = "John", Surname = "Test" };

            // Insert seed data into the database using one instance of the context
            using var context = new ApplicationDbContext(options);
            var wallet1 = new Wallet() { Balance = 20.00M, ApplicationUser = user1 };
            var wallets = new Wallet[] { wallet1 };

            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            var bankerService = new BankerService(context);
            //try to billpay to 1 - kmart
            await bankerService.BpayTransfer(wallet1, 7M, 1, "my billpay posotive", Currency.Aud, TransactionType.BPay);
        }

    }
}
