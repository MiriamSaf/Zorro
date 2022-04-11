using System.ComponentModel.DataAnnotations;

namespace Zorro.WebApplication.ViewModels
{
    public class BpayRequestViewModel
    {
        public string BillPayID  { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";

        [Range(0.000001, double.MaxValue, ErrorMessage = "Amount must be posotive")]
        public decimal Amount { get; set; }
        public int PayeeId { get; internal set; }
    }
}
