using System.Security.Claims;

namespace Zorro.WebApplication.Dtos
{
    public class DepositRequestDto
    {
        public static ClaimsPrincipal User { get; private set; }
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";

        public String CreditCardNumber { get; set; }
        public String CCExpiry { get; set; }
    }
}
