using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class UserCallLog
    {
        [Key,Required]
        public int CallLogID { get; set; }
        [Required]
        public string CallStartTime { get; set; }
        [Required]
        public string CallEndTime { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
