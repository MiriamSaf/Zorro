using System.ComponentModel.DataAnnotations;

namespace Zorro.Dal.Models
{
    public class Shops
    {
        [Key, Required]
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public int Ordering { get; set; }
    }
}
