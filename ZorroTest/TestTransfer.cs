using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zorro.Dal;
using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;
using Zorro.WebApplication.Data;
using System.Threading.Tasks;

namespace ZorroTest
{
    
    [TestClass]
    public class TestTransfer
    {
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
            await bankerService.TransferFunds(wallet1, wallet2, 11.00M, "");
        }
        
    }
}
