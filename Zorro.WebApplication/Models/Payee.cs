using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class Payee
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayeeId { get; set; }
        public string BusinessName { get; set; }
        public decimal Amount { get; set; }

        public DateTime ScheduleTimeUTC { get; set; }

        public string PaymentFrequency { get; set; }


    }
}
