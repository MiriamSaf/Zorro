using Zorro.Dal.Models;

namespace Zorro.WebApplication.ViewModels
{
    //user admin view model class with list of users and merchants
    public class UserAdminViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public List<Merchant> Merchants { get; set; } = new();
    }
}
