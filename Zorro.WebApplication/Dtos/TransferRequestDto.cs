namespace Zorro.WebApplication.Dtos
{
    public class TransferRequestDto
    {
        public string RecipientId { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
