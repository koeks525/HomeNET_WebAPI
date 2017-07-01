
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HouseProfileImage
    {
        [Required]
        public int HouseProfileImageID { get; set; }
        [Required]
        public string HouseImage { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        [ForeignKey("HouseID")]
        public int HouseID { get; set; }
        public House House { get; set; }

    }
}
