namespace Zorro.WebApplication.Dtos
{
    public class TransferRequestDto
    {
        public string RecipientWallet { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
