using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Zorro.WebApplication.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Zorro.WebApplication.ViewModels

{
    public class UserViewModel
    {
        [StringLength(50, ErrorMessage = "Error: First Name has a maximum of 50 Characters.")]
        public string FirstName { get; set; }

        [StringLength(60, ErrorMessage = "Error: Surname has a maximum of 60 Characters.")]
        public string Surname { get; set; }
        public DateTime? BirthDate { get; set; }

        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
        public string Mobile { get; set; }

    }
}
