using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HouseAnnouncement
    {
        [Key,Required]
        public int HouseAnnouncementID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("HouseID")]
        public int HouseID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public House House { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}
