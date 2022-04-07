using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zorro.WebApplication.Data;

namespace Zorro.WebApplication.Models
{
    //account model
    public class Wallet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Display(Name = "Wallet ID")]
        public Guid Id { get; set; }

        [Required, ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        [NotMapped]
        public string DisplayName => ApplicationUser.NormalizedEmail;

        [Required, Range(0, float.MaxValue, ErrorMessage = "Error: Please enter a number that is positive.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}