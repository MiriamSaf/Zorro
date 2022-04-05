using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zorro.WebApplication.Data;

namespace Zorro.WebApplication.Models
{
    //account model
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Display(Name = "Account Number")]
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
        public int AccountNumber { get; set; }

        [Required, ForeignKey("ApplicationUser")]
        public string CustomerID { get; set; }

        [Required, Range(0, float.MaxValue, ErrorMessage = "Error: Please enter a number that is positive.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required, Range(0, 4, ErrorMessage = "Error: Invalid Free Transactions Number")]
        public int FreeTransactions { get; set; }

        public virtual List<Transaction> Transactions { get; set; }


        public ApplicationUser ApplicationUser { get; set; }

    }
}