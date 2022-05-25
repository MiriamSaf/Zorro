namespace Zorro.WebApplication.ViewModels
{
    //transfer result view model with transfer fields
    public class TransferResultViewModel
    {
        public TransferResultViewModelStatus Status { get; set; }
        public string TransactionID { get; set; }
        public string RecipientDisplayName { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }

    //the possible status's for a transfer stored in an enum
    public enum TransferResultViewModelStatus
    {
        Approved,
        InvalidRecipient,
        InsufficientFunds,
        InvalidAmount
    }
}
