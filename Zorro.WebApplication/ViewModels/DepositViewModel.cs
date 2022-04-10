namespace Zorro.WebApplication.ViewModels
{
    public class DepositViewModel

    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }

        public Guid Id { get; set; }
    }
}
