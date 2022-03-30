namespace Zorro.WebApplication.ViewModels
{
    public class DashboardViewModel
    {
        public string WalletId { get; set; } = "";
        public decimal Balance { get; set; }
        public List<TransactionViewModel> RecentTransactions { get; set; } = new();
    }
}
