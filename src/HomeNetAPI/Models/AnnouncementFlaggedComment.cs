using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class AnnouncementFlaggedComment
    {
        [Key]
        public int FlaggedCommentID { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public string DateFlagged { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}
