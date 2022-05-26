using Microsoft.EntityFrameworkCore;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Data
{
    //banker service class 
    public class BankerService : IBanker
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BankerService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        //gets a wallet by its display name 
        public async Task<Wallet> GetWalletByDisplayName(string displayName)
        {
            string normalizedDisplayName = displayName.ToUpper();
            var result = _applicationDbContext.Wallets
                .Include(x => x.ApplicationUser)
                .Where(y => y.ApplicationUser.NormalizedEmail == normalizedDisplayName);
            //returns the wallet that matches 
            return await result.FirstOrDefaultAsync();
        }

        //transfer funds from source to destination
        public async Task<Guid> TransferFunds(string sourceWalletId, string destinationWalletId,
            decimal amount, string comment, Currency currency = Currency.Aud,
            TransactionType transactionType = TransactionType.Transfer)
        {
            var sourceWallet = await GetWalletByDisplayName(sourceWalletId);
            if (sourceWallet is null)
                throw new InvalidWalletException($"{sourceWalletId} is not a valid wallet");
            var destinationWallet = await GetWalletByDisplayName(destinationWalletId);
            if (destinationWallet is null)
                throw new InvalidWalletException($"{destinationWalletId} is not a valid wallet");

            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains('.') && transactionType == TransactionType.Transfer)
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                if (decimals >= 3)
                {
                    throw new InvalidTransferAmountException("Transfer amount cannot have more than 2 decimal places"); 
                }
            }
            // verify that payment can proceed
            if (transactionType == TransactionType.Transfer && amount == 0)
            {
                throw new InvalidTransferAmountException("Transfer amount must not be zero");
            }
            //check if posotive amt
            if (transactionType == TransactionType.Transfer && amount < 0)
            {
                throw new InvalidTransferAmountException("Transfer amount must be a positive number");
            }
            //check that refund is not negative 
            if (transactionType == TransactionType.Refund && amount > 0)
            {
                throw new InvalidTransferAmountException("Refund amount must be a negative number");
            }
            //check has sufficient funds in wallet 
            if (sourceWallet.Balance < amount)
                throw new InsufficientFundsException("The sender has insufficient funds");

            var now = DateTime.Now;
            //all pass so create a source transaction
            var sourceTransaction = new Transaction()
            {
                Amount = amount * -1,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = sourceWallet
            };
            //destination transaction
            var destinationTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = Currency.Aud,
                TransactionTimeUtc = now,
                TransactionType = TransactionType.Transfer,
                Wallet = destinationWallet
            };
            //add source trnsaction
            await _applicationDbContext.AddAsync(sourceTransaction);
            sourceWallet.Balance += sourceTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);
            //add destination transaction
            await _applicationDbContext.AddAsync(destinationTransaction);
            destinationWallet.Balance += destinationTransaction.Amount;
            _applicationDbContext.Wallets.Update(sourceWallet);

            await _applicationDbContext.SaveChangesAsync();
            return sourceTransaction.Id;
        }

        //depsit funds method
        public async Task<Guid> DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment)
        {
            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains('.') && transactionType == TransactionType.Payment)
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                if (decimals >= 3)
                {
                    throw new InvalidDepositAmountException("Deposit amount cannot have more than 2 decimal places");
                }
            }
            // verify that deposit can proceed
            if (transactionType == TransactionType.Payment && amount < 0)
            {
                throw new InvalidDepositAmountException("Deposit amount must not be negative");
            }
            // verify that payment can proceed
            if (transactionType == TransactionType.Payment && amount == 0)
            {
                throw new InvalidDepositAmountException("Deposit amount must not be zero");
            }
            
            var now = DateTime.Now;
            //add deposit transaction
            var depositTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transactionType,
                Wallet = destinationWallet
            };

            destinationWallet.Balance += amount;
            await _applicationDbContext.AddAsync(depositTransaction);

            //save changes to DB
            await _applicationDbContext.SaveChangesAsync();
            return depositTransaction.Id;
        }


        //get transactions by id
        public async Task<List<Transaction>> GetTransactionsByWallet(Guid walletId)
        {
            var results = await _applicationDbContext
                .Transactions
                .Where(x => x.WalletId == walletId).ToListAsync();

            return results;
        }
        //verify users balance
        public async Task<bool> VerifyBalance(Guid walletId, decimal amount)
        {
            var result = await _applicationDbContext.Wallets.FindAsync(walletId);
            return result.Balance >= amount;
        }

        //bpay transfer function
        public async Task<bool> BpayTransfer(string sourceWalletId, decimal amount, int BpayBillerCode, string comment, Currency currency, TransactionType transaction)
        {
            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains('.') && transaction == TransactionType.BPay)
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                //if decimal is more than 3 throw error
                if (decimals >= 3)
                {
                    throw new InvalidBillPayAmountException("BillPay amount cannot have more than 2 decimal places");
                }
            }
            //if comment is too long
            if(comment.Length > 50)
            {
                throw new Exception("Cannot have comment longer than 50 characters");
            }
            // verify that billpay can proceed
            if (transaction == TransactionType.BPay && amount < 0)
            {
                throw new InvalidBillPayAmountException("BillPay amount must not be negative");
            }
            // verify that deposit can proceed
            if (transaction == TransactionType.BPay && amount == 0)
            {
                throw new InvalidBillPayAmountException("BillPay amount must not be zero");
            }
            var sourceWallet = await GetWalletByDisplayName(sourceWalletId);
            if (sourceWallet is null)
                throw new InvalidWalletException($"{sourceWalletId} is not a valid wallet");
            //cannot transfer more than have in balance
            if (amount >= sourceWallet.Balance)
            {
                throw new InvalidBillPayAmountException("BillPay amount must not be more than in wallet");
            }

            var now = DateTime.Now;
            //create bpay transaction
            var bpayTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment + " " + BpayBillerCode,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transaction,
                Wallet = sourceWallet
            };

            //minus amount from balance
            sourceWallet.Balance -= amount;

            await _applicationDbContext.AddAsync(bpayTransaction);

            await _applicationDbContext.SaveChangesAsync();
            return true;
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

    //add exceptions that are custom for their types 
    public class InvalidBillPayAmountException : Exception
    {
        public InvalidBillPayAmountException()
        {
        }

        public InvalidBillPayAmountException(string message)
            : base(message)
        {
        }

    }

    public class InvalidWalletException : Exception
    {
        public InvalidWalletException()
        {
        }

        public InvalidWalletException(string message)
            : base(message)
        {
        }
    }

    public class InvalidDepositAmountException : Exception
    {
        public InvalidDepositAmountException()
        {
        }

        public InvalidDepositAmountException(string message)
            : base(message)
        {
        }

    }

    public class InvalidTransferAmountException : Exception
    {
        public InvalidTransferAmountException()
        {
        }

        public InvalidTransferAmountException(string message)
            : base(message)
        {
        }

    }

    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {
        }
        public InsufficientFundsException(string message)
            : base(message)
        {
        }
    }


}
