using System.ComponentModel.DataAnnotations.Schema;

namespace Zorro.Dal.Models
{
    //chat message model with chat message fields
    public class ChatMessage
    {
        public int Id { get; set; }
        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        [ForeignKey("Recipient")]
        public string RecipientId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public ApplicationUser Sender { get; set; }
        public ApplicationUser Recipient { get; set; }
    }
}
