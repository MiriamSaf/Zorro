using Microsoft.EntityFrameworkCore;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.Api.Services
{
    public class BusinessBankerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusinessBankerService> _logger;
        private const decimal _currenConversionFee = 0.01M;

        public BusinessBankerService(ApplicationDbContext applicationDBContext, ILogger<BusinessBankerService> logger)
        {
            _context = applicationDBContext;
            _logger = logger;
        }

        private static decimal ConvertCurrency(decimal amount, Currency currencyType)
            => amount * CurrencyRateMap.GetConversionRate(currencyType);

        public async Task<Guid> ProcessPayment(string merchantWalletId, string customerWalletId,
            decimal amount, Currency currencyType, string comment)
        {
            var now = DateTime.Now;
            var customerWallet = await GetWalletByDisplayName(customerWalletId);
            var merchantWallet = await GetWalletByDisplayName(merchantWalletId);

            // convert currency and determine fee (if applicable)
            var convertedAmount = ConvertCurrency(amount, currencyType);
            decimal fee = new();
            if (currencyType != Currency.Aud)
                fee = convertedAmount * _currenConversionFee;

            if (customerWallet.Balance < (convertedAmount + fee))
                throw new Exception("insufficient funds");

            var transactionComment = comment;
            if (currencyType != Currency.Aud)
                transactionComment += $" ({Currency.Aud}{amount} + 1%)";

            var customerTX = new Transaction()
            {
                Amount = amount * -1,
                Comment = transactionComment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = customerWallet
            };

            var merchantTX = new Transaction()
            {
                Amount = amount,
                Comment = transactionComment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = merchantWallet
            };
            return await CommitTransactions(customerTX, merchantTX);
        }

        private async Task<Guid> CommitTransactions(Transaction sourceTransaction, Transaction destionationTransaction)
        {
            await _context.AddAsync(sourceTransaction);
            sourceTransaction.Wallet.Balance += sourceTransaction.Amount;
            _context.Wallets.Update(sourceTransaction.Wallet);

            await _context.AddAsync(destionationTransaction);
            destionationTransaction.Wallet.Balance += destionationTransaction.Amount;
            _context.Wallets.Update(destionationTransaction.Wallet);

            await _context.SaveChangesAsync();
            return destionationTransaction.Id;
        }

        public Guid RefundPayment(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        private async Task<Wallet> GetWalletByDisplayName(string displayName)
        {
            string normalizedDisplayName = displayName.ToUpper();
            var result = await _context.Wallets
                .Include(x => x.ApplicationUser)
                .Where(y => y.ApplicationUser.NormalizedEmail == normalizedDisplayName)
                .FirstOrDefaultAsync();
            if (result is null)
                throw new WalletNotFoundException($"Unable to find wallet with ID {normalizedDisplayName}");

            return result;
        }

        public class WalletNotFoundException : Exception
        {
            public WalletNotFoundException()
            {
            }

            public WalletNotFoundException(string message)
                : base(message)
            {
            }
        }
    }
}
