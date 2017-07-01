using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HousePostFlag
    {
        [Key,Required]
        public int HousePostFlagID { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string DateFlagged { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        public string ResponseMessage { get; set; }
        public int IsFlagged { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        [ForeignKey("HousePostID")]
        public int HousePostID { get; set; }
        public HouseMember HouseMember { get; set; }
        public HousePost HousePost { get; set; }


    }
}
