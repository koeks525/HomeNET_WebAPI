using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class AnnouncementComment
    {
        [Key, Required]
        public int AnnouncementCommentID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string DateAdded { get; set;}
        [Required]
        public int IsDeleted { get; set; }
        [Required]
        public int IsFlagged { get; set; }
        [ForeignKey("HouseAnnouncementID")]
        public int HouseAnnouncementID { get; set; }
        public HouseAnnouncement HouseAnnouncement { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}
