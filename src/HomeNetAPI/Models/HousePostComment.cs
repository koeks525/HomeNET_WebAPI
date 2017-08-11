using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Models
{
    public class HousePostComment
    {
        [Required]
        public int HousePostCommentID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        [ForeignKey("HousePostID")]
        public int HousePostID { get; set; }
        [Required]
        public String Comment { get; set; }
        [Required]
        public String DatePosted { get; set; }
        public int IsDeleted { get; set; } = 0;
        public int IsFlagged { get; set; }
        public HouseMember HouseMember { get; set; }
        public HousePost HousePost { get; set; }

    }
}
