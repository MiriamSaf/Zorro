using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zorro.WebApplication.Data;

namespace Zorro.WebApplication.Models
{
    public class Transaction
    {
        //transaction id - each unique
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TransactionID { get; set; }
        public TransactionType TransactionType { get; set; }

        [ForeignKey("DestinationAccount")]
        public string DestinationAccountId { get; set; }
        public virtual Account DestinationAccount { get; set; }

        public decimal Amount { get; set; }
        public DateTime TransactionTimeUTC { get; set; }

        public CurrencyType CurrencyType { get; set; }

        public string Comment { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }

    public enum TransactionType
    {
        Payment,
        Refund
    }

    public enum TransactionStatus
    {
        Approved,
        Declined,
        Pending
    }

    public enum CurrencyType
    {
        USD = 1,
        Euro = 2,
        Yen = 3,
        Pound = 4,
        CAD = 5,
        Franc = 6
    }

}