namespace Zorro.WebApplication.ViewModels
{
    public class TransactionViewModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
