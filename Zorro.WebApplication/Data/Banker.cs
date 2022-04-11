using Microsoft.EntityFrameworkCore;
using Zorro.WebApplication.Models;

namespace Zorro.WebApplication.Data
{
    public interface IBanker
    {
        Task BpayTransfer(Wallet sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency = Currency.Aud, TransactionType transaction = TransactionType.BPay);
        Task DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment);

        Task<Wallet> GetWalletByDisplayName(string displayName);
        Task TransferFunds(Wallet sourceWallet, Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud, TransactionType transactionType = TransactionType.Transfer);
        Task<List<Transaction>> GetTransactionsByWallet(Guid walletId);
        Task<bool> VerifyBalance(Guid walletId, decimal amount);
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

            return await result.FirstOrDefaultAsync();
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

        public async Task DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment)
        {
            var now = DateTime.Now;
            var depositTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transactionType,
                Wallet = destinationWallet
            };

            if (amount > 0)
            {
                destinationWallet.Balance += amount;

                await _applicationDbContext.AddAsync(depositTransaction);

            }
            await _applicationDbContext.SaveChangesAsync();
        }



        public async Task<List<Transaction>> GetTransactionsByWallet(Guid walletId)
        {
            var results = await _applicationDbContext
                .Transactions
                .Where(x => x.WalletId == walletId).ToListAsync();

            return results;
        }

        public async Task<bool> VerifyBalance(Guid walletId, decimal amount)
        {
            var result = await _applicationDbContext.Wallets.FindAsync(walletId);
            return result.Balance >= amount;
        }

        public async Task BpayTransfer(Wallet sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency, TransactionType transaction)
        {
            var now = DateTime.Now;
            var bpayTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment + " "+ BpayBillerCode,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transaction,
                Wallet = sourceWallet

            };

            if (amount > 0)
            {
                //minus amount from balance
                sourceWallet.Balance -= amount;

                await _applicationDbContext.AddAsync(bpayTransaction);

            }
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
