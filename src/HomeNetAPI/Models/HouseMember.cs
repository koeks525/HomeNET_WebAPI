using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HouseMember
    {
        [Key,Required]
        public int HouseMemberID { get; set; }
        [Required]
        public string DateApplied { get; set; }
        public string DateApproved { get; set; }
        public int ApprovalStatus { get; set; }
        public string DateLeft { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("HouseID")]
        public int HouseID { get; set; }
        [ForeignKey("Id")]
        public int UserID { get; set; }
        public House House { get; set; }
        public User User { get; set; }

    }
}
