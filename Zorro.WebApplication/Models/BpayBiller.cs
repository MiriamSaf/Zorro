using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class BpayBiller
    {
        [Key, Required]
        public int BillerCode { get; set; }
        public string BillerName { get; set; }
    }
}
