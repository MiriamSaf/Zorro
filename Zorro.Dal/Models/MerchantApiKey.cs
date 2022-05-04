using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zorro.Dal.Models
{
    public class MerchantApiKey
    {
        [Key]
        public string ApiKey { get; set; }
        public DateTime Expiry { get; set; } = DateTime.Now.AddYears(1);
        [Required, ForeignKey("Merchant")]
        public int MerchantId { get; set; }

        public Merchant Merchant { get; set; }
    }
}
