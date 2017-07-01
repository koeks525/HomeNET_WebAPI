using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class GalleryImageCommentRemoval
    {
        [Key,Required]
        public int GalleryImageCommentRemovalID { get; set; }
        [Required]
        public string DateRemoved { get; set; }
        [ForeignKey("GalleryImageCommentID")]
        public int GalleryImageCommentID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public GalleryImageComment GalleryImageComment { get; set; }
        public User User { get; set; }


    }
}
