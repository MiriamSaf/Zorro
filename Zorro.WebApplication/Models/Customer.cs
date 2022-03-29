using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualWalletApp.Models
{
    //customer model
    public class Customer
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
        public int CustomerID { get; set; }

        [Required, StringLength(50, ErrorMessage = "Error: Name has a maximum of 50 Characters.")]
        public string Name { get; set; }

        [RegularExpression(@"^.{9,11}$", ErrorMessage = "Error: Must be 9 to 11 Characters.")]
        public string TFN { get; set; }

        [StringLength(50, ErrorMessage = "Error: Address has a maximum of 50 Characters.")]
        public string Address { get; set; }

        [StringLength(40, ErrorMessage = "Error: Suburb has a maximum of 40 Characters.")]
        public string Suburb { get; set; }

        [RegularExpression(@"^[a-zA-Z]{2,3}$", ErrorMessage = "Error: Must be a 2 or 3 lettered Australian state.")]
        public string State { get; set; }

        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Error: Must be 4 Digits.")]
        public string PostCode { get; set; }

        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Error: Must be 10 Digits.")]
        public string Mobile { get; set; }

        public virtual List<Account> Accounts { get; set; }
    }
}
