using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class MessageThreadMessage
    {
        [Key,Required]
        public int MessageThreadMessageID { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string DateSent { get; set; }
        [ForeignKey("MessageThreadID")]
        public int MessageThreadID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public MessageThread MessageThread { get; set; }
        public HouseMember HouseMember { get; set; }
    }
}
