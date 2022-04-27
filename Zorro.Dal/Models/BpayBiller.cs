using System.ComponentModel.DataAnnotations;

namespace Zorro.Dal.Models
{
    public class BpayBiller
    {
        [Key, Required]
        public int BillerCode { get; set; }
        public string BillerName { get; set; }
    }
}
