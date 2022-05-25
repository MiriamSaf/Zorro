using System.ComponentModel.DataAnnotations;

namespace Zorro.WebApplication.ViewModels
{
    //deposit class with deposit fields 
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
    
    //status's for the deposit view 
    public enum DepResultViewModelStatus
    {
        Approved,
        InvalidRecipient,
        InsufficientFunds,
        InvalidAmount,
        InvalidCard
    }
}
