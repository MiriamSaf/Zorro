using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class BillPay
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BillPayID { get; set; }

        [Required, ForeignKey("Account"), Display(Name = "Account Identifier")]
        public string AccountId { get; set; }
        //virtual allows to be overidden
        public virtual Account Account { get; set; }

        [Required, ForeignKey("Payee")]
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduleTimeUtc { get; set; }

        public string PaymentFrequency { get; set; }

        //was set in table as string but makes more sense as boolean
        public bool BillState { get; set; }

    }
}