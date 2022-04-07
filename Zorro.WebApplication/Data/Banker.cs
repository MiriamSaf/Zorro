using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public interface IBanker
    {
        Task<Wallet> GetWalletByDisplayName(string displayName);
        Task TransferFunds(Wallet sourceWallet, Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud, TransactionType transactionType = TransactionType.Transfer);
        Task<List<Transaction>> GetTransactionsByWallet(Guid walletId);
    }

    public class Banker : IBanker
    {
        private readonly ILogger<Banker> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public Banker(ILogger<Banker> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Wallet> GetWalletByDisplayName(string displayName)
        {
            string normalizedDisplayName = displayName.ToUpper();
            var result = _applicationDbContext.Wallets
                .Include(x => x.ApplicationUser)
                .Where(y => y.ApplicationUser.NormalizedEmail == normalizedDisplayName);
            if (!result.Any())
                throw new Exception($"Unable to find recipient wallet {normalizedDisplayName}");
            return await result.FirstAsync();
        }

        public async Task TransferFunds(Wallet sourceWallet, Wallet destinationWallet,
            decimal amount, string comment, Currency currency = Currency.Aud,
            TransactionType transactionType = TransactionType.Transfer)
        {
            var now = DateTime.Now;
            var sourceTransaction = new Transaction()
            {
                Amount = amount * -1,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = sourceWallet
            };

            var destinationTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = destinationWallet
            };

            await _applicationDbContext.AddAsync(sourceTransaction);
            sourceWallet.Balance += sourceTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);

            await _applicationDbContext.AddAsync(destinationTransaction);
            destinationWallet.Balance += destinationTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);

            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByWallet(Guid walletId)
        {
            var results = await _applicationDbContext
                .Transactions
                .Where(x => x.WalletId == walletId).ToListAsync();

            return results;
        }
    }
}
