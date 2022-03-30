using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.WebApplication.Models
{
    public class BillPay
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BillPayID { get; set; }

        [Required, ForeignKey("Account"), Display(Name = "Account Number")]
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
        public int AccountNumber { get; set; }
        //virtual allows to be overidden
        public virtual Account Account { get; set; }

        //must make foreign after creates payee
        public int PayeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ScheduleTimeUtc { get; set; }

        public String PaymentFrequency { get; set; }

        //was set in table as string but makes more sense as boolean
        public bool BillState { get; set; }

    }
}
