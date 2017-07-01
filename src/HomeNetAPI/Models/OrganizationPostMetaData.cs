using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HomeNetAPI.Models
{
    public class OrganizationPostMetaData
    {
        [Key,Required]
        public int OrganizationPostMetaDataID { get; set; }
        [ForeignKey("OrganizationPostID")]
        public int OrganizationPostID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        [Required]
        public string DateRecorded { get; set; }

        public int IsDeleted { get; set; }
        public User User { get; set; }
        public OrganizationPost OrganizationPost { get; set; }

    }
}
