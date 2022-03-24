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
