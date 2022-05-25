namespace Zorro.WebApplication.ViewModels
{
    //dashboard class
    public class DashboardViewModel
    {
        public string WalletId { get; set; } = "";
        public decimal Balance { get; set; }
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();
    }
}
