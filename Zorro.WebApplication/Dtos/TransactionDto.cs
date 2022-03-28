namespace Zorro.WebApplication.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public string RecipientId { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
