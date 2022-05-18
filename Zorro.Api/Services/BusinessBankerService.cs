using Microsoft.EntityFrameworkCore;
using Zorro.Api.Exceptions;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public class BusinessBankerService : IBusinessBankerService
    {
        private readonly ILogger<BusinessBankerService> _logger;
        private const decimal _conversionFee = 0.01M;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BusinessBankerService(ILogger<BusinessBankerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private static decimal ConvertCurrency(decimal amount, Currency currencyType)
            => amount * CurrencyRateMap.GetConversionRate(currencyType);

        public async Task<Guid> ProcessPayment(string merchantWalletId, string customerWalletId,
            decimal amount, Currency currencyType, string comment)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var now = DateTime.Now;
            var customerWallet = await GetWalletByDisplayName(customerWalletId, context);
            var merchantWallet = await GetWalletByDisplayName(merchantWalletId, context);

            // convert currency and determine fee (if applicable)
            var convertedAmount = ConvertCurrency(amount, currencyType);
            decimal fee = new();
            if (currencyType != Currency.Aud)
            {
                fee = convertedAmount * _conversionFee;
                convertedAmount += fee;
            }

            if (customerWallet.Balance < (convertedAmount + fee))
                throw new Exception("insufficient funds");

            var transactionComment = comment;
            if (currencyType != Currency.Aud)
                transactionComment += $" ({currencyType}{amount} + 1%)";

            var customerTX = new Transaction()
            {
                Amount = convertedAmount * -1,
                Comment = transactionComment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = customerWallet
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
