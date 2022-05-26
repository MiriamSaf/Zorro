using Microsoft.EntityFrameworkCore;
using Zorro.Dal.Models;
using Zorro.Dal;

namespace Zorro.WebApplication.Data
{
    //banker class has all essential banker services 
    public interface IBanker
    {
        //lits of all the different tasks of the banker
        Task<bool> BpayTransfer(string sourceWallet, decimal amount, int BpayBillerCode, string comment, Currency currency = Currency.Aud, TransactionType transaction = TransactionType.BPay);
        Task<Guid> DepositFunds(Wallet destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud,
          TransactionType transactionType = TransactionType.Payment);

        Task<Wallet> GetWalletByDisplayName(string displayName);
        Task<Guid> TransferFunds(string sourceWallet, string destinationWallet, decimal amount, string comment, Currency currency = Currency.Aud, TransactionType transactionType = TransactionType.Transfer);
        Task<List<Transaction>> GetTransactionsByWallet(Guid walletId);
        Task<bool> VerifyBalance(Guid walletId, decimal amount);
        Task ShopPurchase(Wallet sourceWallet, decimal amount);
    }
}
