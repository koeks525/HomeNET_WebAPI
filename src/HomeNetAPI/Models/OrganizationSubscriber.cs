using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class OrganizationSubscriber
    {
        [Key, Required]
        public int OrganizationSubscriberID { get; set; }
        [ForeignKey("OrganizationID")]
        public int OrganizationID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        public Organization Organization { get; set; }
        public User User { get; set; }

    }
}
