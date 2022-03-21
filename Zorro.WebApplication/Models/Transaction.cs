namespace Zorro.WebApplication.Models
{
    public class Transaction
    {
        public Guid ID { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
    }

    public enum TransactionType
    {
        Payment,
        Refund
    }
}
