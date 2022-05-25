using System.Security.Claims;

namespace Zorro.WebApplication.Dtos
{
    //deposit request data transfer object
    public class DepositRequestDto
    {
        public static ClaimsPrincipal User { get; private set; }
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";

        public string CreditCardNumber { get; set; }
        public string CCExpiry { get; set; }
    }
}
