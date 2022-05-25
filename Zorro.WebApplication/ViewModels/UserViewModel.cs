namespace Zorro.WebApplication.ViewModels

{
    //user view model class with user fields
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool AdminAccessGranted { get; set; }
    }
}
