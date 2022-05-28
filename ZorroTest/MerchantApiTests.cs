using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zorro.Api.Exceptions;
using Zorro.Api.Services;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace ZorroTest
{
    [TestClass]
    public class MerchantApiTests
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactory;
        private readonly BusinessBankerService businessBankerService;

        // initialize depenencies to mock DI requirements for BusinessBankerService
        public MerchantApiTests()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(databaseName: "FakeDB"));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // serviceScopeMock will contain my ServiceProvider
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.SetupGet(s => s.ServiceProvider)
                .Returns(serviceProvider);

            // serviceScopeFactoryMock will contain my serviceScopeMock
            _serviceScopeFactory = new Mock<IServiceScopeFactory>();
            _serviceScopeFactory.Setup(s => s.CreateScope())
                .Returns(serviceScopeMock.Object);
            businessBankerService = new BusinessBankerService(_serviceScopeFactory.Object);
        }

        // Test attempting a transaction with an unsupported currency type
        [ExpectedException(typeof(InvalidCurrencyTypeException))]
        [TestMethod]
        public async Task TestInvalidCurrencyType()
        {
            // create mock databse records
            var merchant = new ApplicationUser()
            {
                FirstName = "Eugene",
                Surname = "Krabs",
                NormalizedEmail = "EUGENE.KRABS@HOTMAIL.COM",
                NormalizedUserName = "EUGENE.KRABS@HOTMAIL.COM",
            };
            var customer = new ApplicationUser()
            {
                FirstName = "Bruce",
                Surname = "Wayne",
                NormalizedEmail = "BRUCE.WAYNE@HOTMAIL.COM",
                NormalizedUserName = "BRUCE.WAYNE@HOTMAIL.COM",
            };
            var merchantWallet = new Wallet() { ApplicationUser = merchant };
            var customerWallet = new Wallet() { Balance = 20.00M, ApplicationUser = customer };
            var wallets = new Wallet[] { merchantWallet, customerWallet };

            using var scope = _serviceScopeFactory.Object.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            // process transaction with an unsupported currency code
            await businessBankerService.ProcessPayment(
                merchantWallet.DisplayName, customerWallet.DisplayName, 20.00M, 9, "Crabby Patty"
                );
        }

        [ExpectedException(typeof(InsufficientFundsException))]
        [TestMethod]
        public async Task TestInsufficientFunds()
        {
            // create mock databse records
            var merchant = new ApplicationUser()
            {
                FirstName = "Eugene",
                Surname = "Krabs",
                NormalizedEmail = "EUGENE.KRABS@HOTMAIL.COM",
                NormalizedUserName = "EUGENE.KRABS@HOTMAIL.COM",
            };
            var customer = new ApplicationUser()
            {
                FirstName = "Bruce",
                Surname = "Wayne",
                NormalizedEmail = "BRUCE.WAYNE@HOTMAIL.COM",
                NormalizedUserName = "BRUCE.WAYNE@HOTMAIL.COM",
            };
            var merchantWallet = new Wallet() { ApplicationUser = merchant };
            var customerWallet = new Wallet() { Balance = 20.00M, ApplicationUser = customer };
            var wallets = new Wallet[] { merchantWallet, customerWallet };

            using var scope = _serviceScopeFactory.Object.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            // Process a transaction that would exceed the customer's balance
            await businessBankerService.ProcessPayment(
                merchantWallet.DisplayName, customerWallet.DisplayName, 21.00M, 0, "Crabby Patty"
                );
        }

        [TestMethod]
        public async Task TestSuccessfulForeignCurrencyTransaction()
        {
            // create mock databse records
            var merchant = new ApplicationUser()
            {
                FirstName = "Eugene",
                Surname = "Krabs",
                NormalizedEmail = "EUGENE.KRABS@HOTMAIL.COM",
                NormalizedUserName = "EUGENE.KRABS@HOTMAIL.COM",
            };
            var customer = new ApplicationUser()
            {
                FirstName = "Bruce",
                Surname = "Wayne",
                NormalizedEmail = "BRUCE.WAYNE@HOTMAIL.COM",
                NormalizedUserName = "BRUCE.WAYNE@HOTMAIL.COM",
            };
            var merchantWallet = new Wallet() { ApplicationUser = merchant };
            var customerWallet = new Wallet() { Balance = 20.00M, ApplicationUser = customer };
            var wallets = new Wallet[] { merchantWallet, customerWallet };

            using var scope = _serviceScopeFactory.Object.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Wallets.AddRangeAsync(wallets);
            context.SaveChanges();

            // intermediate and expected figures
            var customerBalanceBeforeTransaction = customerWallet.Balance;
            decimal amount = 11.49M;
            int currencyCode = (int)Currency.Usd;
            decimal convertedAmount = BusinessBankerService.ConvertCurrency(amount, Currency.Usd);
            decimal fee = Math.Round(convertedAmount * BusinessBankerService.conversionFee, 2);
            decimal expectedCustomerBalance = customerBalanceBeforeTransaction - (convertedAmount + fee);
            decimal expetedMerchantbalance = convertedAmount;

            // Process a transaction that would exceed the customer's balance
            await businessBankerService.ProcessPayment(
                merchantWallet.DisplayName, customerWallet.DisplayName, amount, currencyCode, "Crabby Patty"
                );
            Assert.AreEqual(expectedCustomerBalance, customerWallet.Balance);
            Assert.AreEqual(convertedAmount, merchantWallet.Balance);
        }



    }
}
