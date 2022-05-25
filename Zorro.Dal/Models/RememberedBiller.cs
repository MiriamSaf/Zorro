using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.Dal.Models
{
    //remembered biller model for remembering the billers that a user has saved 
    public class RememberedBiller
    {
        [Required, ForeignKey("BpayBiller")]
        public int BillerCode { get; set; }
        [Required, ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public BpayBiller BpayBiller { get; set; }
    }
}
