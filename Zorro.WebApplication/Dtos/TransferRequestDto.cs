namespace Zorro.WebApplication.Dtos
{
    //transfer data transfer object used for transfer
    public class TransferRequestDto
    {
        public string RecipientWallet { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
