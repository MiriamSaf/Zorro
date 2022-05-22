using System.ComponentModel.DataAnnotations;

namespace Zorro.WebApplication.ViewModels
{
    public class DepositViewModel

    {
        public DepResultViewModelStatus Status { get; set; }
        public string TransactionID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public string CCExpiry { get; set; }
    }

    public enum DepResultViewModelStatus
    {
        Approved,
        InvalidRecipient,
        InsufficientFunds,
        InvalidAmount,
        InvalidCard
    }
}
