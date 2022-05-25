namespace Zorro.WebApplication.ViewModels
{
    //shop dashboard view model with shop field as a list
    public class ShopDashboardViewModel
    {
        public List<ShopViewModel> Shops { get; set; } = new();
    }
}