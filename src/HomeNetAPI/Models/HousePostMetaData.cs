using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class HousePostMetaData
    {
        [Key,Required]
        public int HousePostMetaDataID { get; set; }
        [ForeignKey("HousePostID")]
        public int HousePostID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public int Liked { get; set; }
        public int Disliked { get; set; }
        [Required]
        public string DateAdded { get; set;}
        [Required]
        public int IsDeleted { get; set; }
        public HousePost HousePost { get; set; }
        public User User { get; set; }
    }
}
