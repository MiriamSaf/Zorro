using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zorro.WebApplication.Data;

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
