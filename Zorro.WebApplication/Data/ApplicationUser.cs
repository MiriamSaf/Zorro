using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Zorro.WebApplication.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity) ]
        public int CustomerID { get; set; }


        [Required, StringLength(50, ErrorMessage = "Error: First Name has a maximum of 50 Characters.")]
        public string FirstName { get; set; }


        [Required, StringLength(60, ErrorMessage = "Error: Surname has a maximum of 60 Characters.")]
        public string Surname { get; set; }
        public string AvatarUrl { get; set; }

        public DateOnly BirthDate { get; set; }


        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
        public int Mobile { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
