using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class Shops
    {
        [Key, Required]
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public int Ordering { get; set; }
    }
}
