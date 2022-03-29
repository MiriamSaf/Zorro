using Zorro.WebApplication.Data;

namespace Zorro.WebApplication.Models
{
    public class Transaction
    {
        public Guid ID { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime CompletedDateTime { get; set; }
        public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;

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
}

/* old transaction model
 using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualWalletApp.Models
{
    //transaction model 

    /* -- Currency Types --
    US dollars 
    Euro 
    Japanese yen 
    British pound 
    Canadian dollar 
    Swiss Franc 
    
public enum CurrencyType
{
    USD = 1,
    Euro = 2,
    Yen = 3,
    Pound = 4,
    CAD = 5,
    Franc = 6
}

public enum TransactionType
{
    Deposit = 1,
    Withdraw = 2,
    Transfer = 3,
    Commission = 4
}
public class Transaction
{
    [Required]
    public int TransactionID { get; set; }

    [Required, RegularExpression(@"^[1-4]$", ErrorMessage = "Error: Invalid Transaction Type")]
    public TransactionType TransactionType { get; set; }

    [Required, ForeignKey("Account"), Display(Name = "Account Number")]
    [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
    public int? DestinationAccountNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Required, Range(0, float.MaxValue, ErrorMessage = "Error: Please enter a number that is positive.")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Error: Comment has a maximum of 30 Characters.")]
    public string Comment { get; set; }

    [Required]
    public DateTime TransactionTimeUtc { get; set; }
}
}
*/
