namespace Zorro.WebApplication.Dtos
{
    public class BpayRequestDto
    {
        public string BillerCode { get; set; } = "";
        public bool RememberBiller { get; set; }
        public string CustomerReferenceNumber { get; set; } = "";
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
    }
}
