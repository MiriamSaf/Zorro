using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.Dal.Models
{
    //wallet model with relevant wallet fields
    public class Wallet
    {
        //the key is the ID
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

        //each wallet has an application user attached to it 
        public ApplicationUser ApplicationUser { get; set; }

    }
}