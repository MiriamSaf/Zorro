using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Look for customers.
            /*if(context.Customers.Any())
                return; // DB has already been seeded.

            context.Customers.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    Name = "Matthew Bolger",
                    TFN = "12345678912",
                    Address = "123 Fake Street",
                    Suburb = "Melbourne",
                    State = "VIC",
                    PostCode = "3000",
                    Mobile = "0412345678"
                });

 /*           context.Logins.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2"
                },
                new Login
                {
                    LoginID = "38074569",
                    CustomerID = 2200,
                    PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04"
                },
                new Login
                {
                    LoginID = "17963428",
                    CustomerID = 2300,
                    PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE"
                });

            context.Accounts.AddRange(
                new Account
                {
                    AccountNumber = 4100,
                    AccountType = AccountType.Saving,
                    CustomerID = 2100,
                    Balance = 100,
                    FreeTransactions = 0
                },
                new Account
                {
                    AccountNumber = 4101,
                    AccountType = AccountType.Checking,
                    CustomerID = 2100,
                    Balance = 500,
                    FreeTransactions = 0
                },
                new Account
                {
                    AccountNumber = 4200,
                    AccountType = AccountType.Saving,
                    CustomerID = 2200,
                    Balance = 500.95m,
                    FreeTransactions = 0
                },
                new Account
                {
                    AccountNumber = 4300,
                    AccountType = AccountType.Checking,
                    CustomerID = 2300,
                    Balance = 1250.50m,
                    FreeTransactions = 0
                });
            
            const string format = "dd/MM/yyyy hh:mm:ss tt";

            context.Transactions.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = "Opening balance",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 200,
                    Comment = "First deposit",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:30:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 300,
                    Comment = "Second deposit",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 08:45:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500,
                    Comment = "Deposited $500",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 09:00:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 0.95m,
                    Comment = "Deposited $0.95",
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 09:15:00 PM", format, null)
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = null,
                    TransactionTimeUtc = DateTime.ParseExact("19/05/2021 10:00:00 PM", format, null)
                });*/

            context.SaveChanges();
        }
    }
}