using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Zorro.Dal.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50, ErrorMessage = "Error: First Name has a maximum of 50 Characters.")]
        public string FirstName { get; set; }

        [StringLength(60, ErrorMessage = "Error: Surname has a maximum of 60 Characters.")]
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; }

        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
        public string Mobile { get; set; }

        [RegularExpression(@"([0-9]{12}(?:[0-9]{3})?)", ErrorMessage = "Enter 12 digits consecutively (not separated by hyphen or comma)")]
        public string CreditCardNumber { get; set; }

        [RegularExpression("^(([1-9][0-2]?)|([0-0][1-9]))/([2-9][0-9]{1})$", ErrorMessage = "Error: Must be 4 Digits with a / in between numbers - as in MM/YY")]
        public string CCExpiry { get; set; }

        public string Avatar { get; set; }

        public string APIKey { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LockoutEnd { get; set; }

    }
}