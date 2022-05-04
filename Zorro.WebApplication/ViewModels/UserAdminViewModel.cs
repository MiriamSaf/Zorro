using Zorro.Dal.Models;

namespace Zorro.WebApplication.ViewModels
{
    public class UserAdminViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public List<Merchant> Merchants { get; set; } = new();
    }
}
