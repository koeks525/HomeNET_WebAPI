using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace HomeNetAPI.Models
{
    public class InterHouseMessageReply
    {
        [Key,Required]
        public int InterHouseMessageReplyID { get; set; }
        [ForeignKey("InterHouseMailID")]
        public int InterHouseMailID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string DateAdded { get; set; }
        [Required]
        public int IsDeleted { get; set; }
        public InterHouseMail InterHouseMail { get; set; }
        public User User { get; set; }


        
    }
}
