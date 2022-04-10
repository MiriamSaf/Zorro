namespace Zorro.WebApplication.Dtos
{
    public class DepositRequestDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
    }
}
