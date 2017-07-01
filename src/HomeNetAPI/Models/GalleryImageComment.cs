using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class GalleryImageComment
    {
        [Key,Required]
        public int GalleryImageCommentID { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        [ForeignKey("GalleryImageID")]
        public int GalleryImageID { get; set; }
        public int IsFlagged { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public GalleryImage GalleryImage { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}
