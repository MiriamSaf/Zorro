using Microsoft.EntityFrameworkCore;
using Zorro.Dal;
using Zorro.Dal.Models;

namespace Zorro.WebApplication.Data
{
    public class BankerService : IBanker
    {
        private readonly ILogger<Banker> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public BankerService(ApplicationDbContext applicationDbContext)
        {
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
            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains(".") && transactionType == TransactionType.Transfer)
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
            if (transactionType == TransactionType.Transfer && amount < 0)
            {
                throw new InvalidTransferAmountException("Transfer amount must be a positive number");
            }
            if (transactionType == TransactionType.Refund && amount > 0)
            {
                throw new InvalidTransferAmountException("Refund amount must be a negative number");
            }
            if (sourceWallet.Balance < amount)
                throw new InsufficientFundsException("The sender has insufficient funds");

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

        //depsit funds method
        public async Task DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment)
        {
            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains(".") && transactionType == TransactionType.Payment)
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

        public async Task<bool> BpayTransfer(Wallet sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency, TransactionType transaction)
        {
            //check for decimals greater than 3 places 
            String checkDec = amount.ToString();
            if (checkDec.Contains(".") && transaction == TransactionType.BPay)
            {
                string[] a = checkDec.Split(new char[] { '.' });
                int decimals = a[1].Length;
                if (decimals >= 3)
                {
                    throw new InvalidBillPayAmountException("BillPay amount cannot have more than 2 decimal places");
                }
            }

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
            var now = DateTime.Now;
            var bpayTransaction = new Transaction()
            {
                Amount = amount,
                Comment = comment + " " + BpayBillerCode,
                CurrencyType = currency,
                TransactionTimeUtc = now,
                TransactionType = transaction,
                Wallet = sourceWallet

            };

            //cannot transfer more than have in balance
            if (amount >= sourceWallet.Balance)
            {

                throw new InvalidBillPayAmountException("BillPay amount must not be more than in wallet");
                //return false;
            }

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
    }

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
