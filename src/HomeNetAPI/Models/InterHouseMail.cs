using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class InterHouseMail
    {
        [Key,Required]
        public int InterHouseMailID { get; set; }
        [ForeignKey("UserID")]
        public int SenderID { get; set; } //The person sending the mail
        [ForeignKey("HouseID")]
        public int HouseID { get; set; } //The house it is being sent to
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; } //Description of the messagr request
        public string Document { get; set; } //Store a reference to the document
        [Required]
        public string DateSent { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        public House House { get; set; }
        public User User { get; set; }
    }
}
