namespace Zorro.WebApplication.ViewModels
{
    //transaction view model with transaction fields
    public class TransactionViewModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
