﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Zorro.WebApplication.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Zorro.WebApplication.Data
{
    public class ApplicationUser : IdentityUser
    {        
        [StringLength(50, ErrorMessage = "Error: First Name has a maximum of 50 Characters.")]
        public string FirstName { get; set; }

        [StringLength(60, ErrorMessage = "Error: Surname has a maximum of 60 Characters.")]
        public string Surname { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? BirthDate { get; set; }

        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
        public string Mobile { get; set; }

        [RegularExpression(@"([0-9]{12}(?:[0-9]{3})?)", ErrorMessage = "Enter 12 digits consecutively (not separated by hyphen or comma)")]
        public string CreditCardNumber { get; set; }

        [RegularExpression(" ^ (0[1-9]|1[0-2])-?([0-9]{4}|[0-9]{2})$", ErrorMessage = "Error: Must be 4 Digits with a / in between numbers")]
        public string CCExpiry { get; set; }

        public int ImageId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }

    }
}
