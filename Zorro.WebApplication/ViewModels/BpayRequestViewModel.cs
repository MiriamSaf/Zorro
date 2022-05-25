using System.ComponentModel.DataAnnotations;

namespace Zorro.WebApplication.ViewModels
{
    //bpay class with bpay fields
    public class BpayRequestViewModel
    {
        public BpayResultViewModelStatus Status { get; set; }
        public string TransactionID { get; set; }
        public int BillPayID  { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";

        [Range(0.000001, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }
        public int PayeeId { get; internal set; }
    }

    //bpay results 
    public enum BpayResultViewModelStatus
    {
        Approved,
        InvalidRecipient,
        InsufficientFunds,
        InvalidAmount
    }
}
