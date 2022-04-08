namespace Zorro.WebApplication.ViewModels
{
    public class TransferResultViewModel
    {
        public TransferResultViewModelStatus Status { get; set; }
        public string RecipientDisplayName { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }

    public enum TransferResultViewModelStatus
    {
        Approved,
        InvalidRecipient,
        InsufficientFunds,
        InvalidAmount
    }
}
