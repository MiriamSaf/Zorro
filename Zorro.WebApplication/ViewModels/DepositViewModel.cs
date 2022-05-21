using System.ComponentModel.DataAnnotations;

namespace Zorro.WebApplication.ViewModels
{
    public class DepositViewModel

    {
        public DepResultViewModelStatus Status { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";

        public decimal Amount { get; set; }

        public Guid Id { get; set; }


        public String CreditCardNumber { get; set; }
        public String CCExpiry { get; set; }
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
