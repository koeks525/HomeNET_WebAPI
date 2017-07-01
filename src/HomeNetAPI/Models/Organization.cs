using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class Organization
    {
        [Key, Required]
        public int OrganizationID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Location { get; set; } //a GPS co-ordinate 
        public string OrganizationPhoto { get; set; }
        [Required]
        public string DateAdded { get; set; }
        public string TelephoneNumber { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public int IsDeleted { get; set; }
        public string SkypeID { get; set; }
        public string TwitterID { get; set; }
        public string FacebookID { get; set; }
        [ForeignKey("CategoryID")]
        public int CategoryID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }
    }
}
