using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class OrganizationPost
    {
        [Key,Required]
        public int OrganizationPostID { get; set; }
        [ForeignKey("OrganizationID")]
        public int OrganizationID { get; set; }
        [Required]
        public string PostText { get; set; }
        public string PostImage { get; set; }
        public string PostVoiceNote { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        public Organization Organization { get; set; }
    }
}
