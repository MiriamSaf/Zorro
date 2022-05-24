using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;
using Zorro.Dal;

namespace Zorro.WebApplication.Data
{
    //banker class has all essential banker services 
    public interface IBanker
    {
        //lits of all the different tasks of the banker
        Task<bool> BpayTransfer(Wallet sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency = Currency.Aud, TransactionType transaction = TransactionType.BPay);
        Task DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment);

        Task<Wallet> GetWalletByDisplayName(string displayName);
        Task TransferFunds(Wallet sourceWallet, Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud, TransactionType transactionType = TransactionType.Transfer);
        Task<List<Transaction>> GetTransactionsByWallet(Guid walletId);
        Task<bool> VerifyBalance(Guid walletId, decimal amount);
        Task ShopPurchase(Wallet sourceWallet, decimal amount);
    }

    public class Banker : IBanker
    {
        private readonly ILogger<Banker> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        //DI for the banker class
        public Banker(ILogger<Banker> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        //gets a wallet by passing in the users display name
        public async Task<Wallet> GetWalletByDisplayName(string displayName)
        {
            //finds the users wallet based on their name 
            string normalizedDisplayName = displayName.ToUpper();
            var result = _applicationDbContext.Wallets
                .Include(x => x.ApplicationUser)
                .Where(y => y.ApplicationUser.NormalizedEmail == normalizedDisplayName);
            //returns users wallet
            return await result.FirstOrDefaultAsync();
        }

        //allows for funds to be transfered from a source to a destination 
        public async Task TransferFunds(Wallet sourceWallet, Wallet destinationWallet,
            decimal amount, string comment, Currency currency = Currency.Aud,
            TransactionType transactionType = TransactionType.Transfer)
        {

            var now = DateTime.Now;
            //create the source transaction
            var sourceTransaction = new Transaction()
            {
                Amount = amount * -1,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = sourceWallet
            };
            //create the destination that the money will go to as a transaction
            var destinationTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = destinationWallet
            };
            //add the source transaction
            await _applicationDbContext.AddAsync(sourceTransaction);
            sourceWallet.Balance += sourceTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);
            //add the destination transaction
            await _applicationDbContext.AddAsync(destinationTransaction);
            destinationWallet.Balance += destinationTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);
            //save the changes
            await _applicationDbContext.SaveChangesAsync();
        }

        //deposit funds function
        public async Task DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment)
        {
            var now = DateTime.Now;
            //add a deposit transaction
            var depositTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transactionType,
                Wallet = destinationWallet
            };

           
            //add additional checks 
            if (amount > 0)
            {
                destinationWallet.Balance += amount;

                await _applicationDbContext.AddAsync(depositTransaction);

            }
            //save the changes 
            await _applicationDbContext.SaveChangesAsync();
        }


        //get transactions by passing in the wallet id 
        public async Task<List<Transaction>> GetTransactionsByWallet(Guid walletId)
        {
            var results = await _applicationDbContext
                .Transactions
                .Where(x => x.WalletId == walletId).ToListAsync();

            return results;
        }

        //verify the users balance by passing in their walletid and an amount
        public async Task<bool> VerifyBalance(Guid walletId, decimal amount)
        {
            var result = await _applicationDbContext.Wallets.FindAsync(walletId);
            return result.Balance >= amount;
        }

        //bpay transfer - to make a bpay
        public async Task<bool> BpayTransfer(Wallet sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency = Currency.Aud, TransactionType transaction = TransactionType.BPay)
        {
            var now = DateTime.Now;
            //create a bpay transaction that will be entered into system
            var bpayTransaction = new Transaction()
            {
                Amount = amount * -1,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.BPay,
                Wallet = sourceWallet

            };

            //cannot transfer more than have in balance
            if (amount >= sourceWallet.Balance)
            {
                return false;
            }
            //amoutn above zero
            if (amount > 0)
            {
                //minus amount from balance
                sourceWallet.Balance -= amount;

                await _applicationDbContext.AddAsync(bpayTransaction);

                await _applicationDbContext.SaveChangesAsync();
                return true;


            }

            return false;
        }

        //make a shop purchase 
        public async Task ShopPurchase(Wallet sourceWallet, decimal amount)
        {
            var now = DateTime.Now;
            //create a shop transaction
            var shopTransaction = new Transaction()
            {
                Amount = amount,
                Comment = "Shop Purchase",
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Shop,
                Wallet = sourceWallet
            };
            //add the transaction into the system
            await _applicationDbContext.AddAsync(shopTransaction);

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
