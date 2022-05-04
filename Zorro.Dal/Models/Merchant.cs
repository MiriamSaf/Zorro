using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zorro.Dal.Models
{
    public class Merchant
    {
        [Key, Required]
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string Abn { get; set; }
        public MerchantStatus Status { get; set; }
        [Required, ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public List<MerchantApiKey> ApiKeys { get; set; }
    }

    public enum MerchantStatus
    {
        Pending,
        Approved
    }
}
