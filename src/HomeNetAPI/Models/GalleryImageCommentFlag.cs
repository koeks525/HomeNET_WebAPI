using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class GalleryImageCommentFlag
    {
        [Key,Required]
        public int ReportID { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string DateFlagged { get; set; }
        [ForeignKey("GalleryImageCommentID")]
        public int GalleryImageCommentID { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public HouseMember HouseMember { get; set; }
        public GalleryImageComment GalleryImageComment { get; set; }

    }
}
