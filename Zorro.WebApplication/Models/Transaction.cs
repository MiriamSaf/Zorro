using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zorro.WebApplication.Data;

namespace Zorro.WebApplication.Models
{
    public class Transaction
    {
        //transaction id - each unique
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public TransactionType TransactionType { get; set; }
        [ForeignKey("Wallet")]
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTimeUtc { get; set; }
        public Currency CurrencyType { get; set; } = Currency.Aud;
        public string Comment { get; set; }
        public virtual Wallet Wallet { get; set; }
    }

    public enum TransactionType
    {
        Transfer,
        Payment,
        Refund,
        BPay
    }

    public enum Currency
    {
        Aud = 0,
        Usd = 1,
        Euro = 2,
        Yen = 3,
        Pound = 4,
        Cad = 5,
        Franc = 6
    }

}