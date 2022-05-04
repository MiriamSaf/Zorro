using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Zorro.Dal.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Zorro.WebApplication.ViewModels

{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public bool LockedOut { get; set; }
        public bool AdminAccessGranted { get; set; }
    }
}
