using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class MessageThreadParticipant
    {
        [Key,Required]
        public int MessageThreadParticipantID { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("MessageThreadID")]
        public int MessageThreadID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public HouseMember HouseMember { get; set; }
        public MessageThread MessageThread { get; set; }

    }
}
