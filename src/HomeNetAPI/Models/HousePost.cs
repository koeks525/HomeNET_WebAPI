using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HousePost
    {
        [Key,Required]
        public int HousePostID { get; set; }

        public string Title { get; set; }
        [Required]
        public string PostText { get; set; }
        [Required]
        public string DatePosted { get; set; }
        
        public string Location { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [Required]
        public int IsFlagged { get; set; } = 0;
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        
        public string MediaResource { get; set; }
        public string ResizedMediaResource { get; set; }

        public HouseMember HouseMember { get; set; }

    }
}
