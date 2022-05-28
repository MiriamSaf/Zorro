using Microsoft.EntityFrameworkCore;
using Zorro.Api.Exceptions;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public class BusinessBankerService : IBusinessBankerService
    {
        public static readonly decimal conversionFee = 0.01M;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BusinessBankerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public static decimal ConvertCurrency(decimal amount, Currency currencyType)
            => Math.Round(amount * CurrencyRateMap.GetConversionRate(currencyType), 2);

        public static decimal GetFee(decimal amount) =>
            Math.Round(amount * conversionFee, 2);

        public async Task<Guid> ProcessPayment(string merchantWalletId, string customerWalletId,
            decimal amount, int currencyTypeInt, string comment)
        {
            // verify currency type
            if (!Enum.IsDefined(typeof(Currency), currencyTypeInt))
                throw new InvalidCurrencyTypeException($"{currencyTypeInt} does't correspond with a supported currency");
            var currencyType = (Currency)currencyTypeInt;

            amount = Math.Round(amount, 2);
            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = DateTime.Now;

            var merchantWallet = await GetWalletByDisplayName(merchantWalletId, context);
            if (merchantWallet is null)
                throw new WalletNotFoundException("Unable to find merchant wallet");
            var customerWallet = await GetWalletByDisplayName(customerWalletId, context);
            if (customerWallet is null)
                throw new WalletNotFoundException("Unable to find customer wallet");

            // convert currency and determine fee (if applicable)
            decimal convertedAmount = ConvertCurrency(amount, currencyType);
            if (convertedAmount < 1)
                throw new InvalidAmountException("Amount must be at least $1.00 AUD");

            decimal fee = new();
            if (currencyType != Currency.Aud)
                fee = GetFee(convertedAmount);

            if (customerWallet.Balance < (convertedAmount + fee))
                throw new InsufficientFundsException("Customer has insufficient funds for payment");

            var transactionComment = comment;
            if (currencyType != Currency.Aud)
                transactionComment += $" ({currencyType}{amount} + 1%)";

            var customerTX = new Transaction()
            {
                Amount = (convertedAmount + fee) * -1,
                Comment = transactionComment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = customerWallet,
            };

            var merchantTX = new Transaction()
            {
                Amount = convertedAmount,
                Comment = transactionComment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = merchantWallet
            };
            return await CommitTransactions(customerTX, merchantTX, context);

        }

        private static async Task<Guid> CommitTransactions(Transaction sourceTransaction, Transaction destionationTransaction, ApplicationDbContext context)
        {
            await context.AddAsync(sourceTransaction);
            sourceTransaction.Wallet.Balance += sourceTransaction.Amount;
            context.Wallets.Update(sourceTransaction.Wallet);

            await context.AddAsync(destionationTransaction);
            destionationTransaction.Wallet.Balance += destionationTransaction.Amount;
            context.Wallets.Update(destionationTransaction.Wallet);

            await context.SaveChangesAsync();
            return destionationTransaction.Id;
        }

        public Guid RefundPayment(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        private static async Task<Wallet> GetWalletByDisplayName(string displayName, ApplicationDbContext context)
        {
            string normalizedDisplayName = displayName.ToUpper();
            var result = await context.Wallets
                .Include(x => x.ApplicationUser)
                .Where(y => y.ApplicationUser.NormalizedEmail == normalizedDisplayName)
                .FirstOrDefaultAsync();
            if (result is null)
                throw new WalletNotFoundException($"Unable to find wallet with ID {normalizedDisplayName}");

            return result;
        }
    }
}
