using System.ComponentModel.DataAnnotations;

namespace Zorro.Dal.Models
{
    //bpay biller model with fields
    public class BpayBiller
    {
        [Key, Required]
        public int BillerCode { get; set; }
        public string BillerName { get; set; }
    }
}
