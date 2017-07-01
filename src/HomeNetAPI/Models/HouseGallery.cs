using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HouseGallery
    {
        [Key,Required]
        public int HouseGalleryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int IsDeleted { get; set; }

        [Required]
        public string Location { get; set; } //This will be GPS data resolved to a city, country, etc. 
        [Required]
        public string DateCreated { get; set; }
        [ForeignKey("HouseMemberID")]
        public int HouseMemberID { get; set; }
        public HouseMember HouseMember { get; set; }

    }
}
