using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.Dal.Models
{
    public class Merchant
    {
        [Key, Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Registered Business Name")]
        public string BusinessName { get; set; }

        [Display(Name = "ABN / ACN")]
        public string Abn { get; set; }
        public MerchantStatus Status { get; set; }
        [Required, ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public List<MerchantApiKey> ApiKeys { get; set; }

        [Display(Name = "Trading Name")]
        public string TradingName { get; set; }

        [Display(Name = "Company Phone Number")]
        public string CompanyPhone { get; set; }

        [Display(Name = "Registered Trading Address")]
        public string TradingAddress { get; set; }

        [Display(Name = "Date Established")]
        public string DateEstablished { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Position (Director/Manager/Owner)")]
        public string ACCPosition { get; set; }

        [Display(Name = "Drivers License Number")]
        public string DriversLicense { get; set; }
    }

    public enum MerchantStatus
    {
        Pending,
        Approved
    }
}
