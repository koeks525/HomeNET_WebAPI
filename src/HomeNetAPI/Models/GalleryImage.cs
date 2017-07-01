using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HomeNetAPI.Models
{
    public class GalleryImage
    {
        [Key,Required]
        public int GalleryImageID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        public string DateAdded { get; set; }
        public string Keywords { get; set; } //Separated by comma's.
        [ForeignKey("HouseGalleryID")]
        public int HouseGalleryID { get; set; }
        public HouseGallery HouseGallery { get; set; }
    }
}
