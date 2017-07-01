using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class CallLogRecepients
    {
        [Key, Required]
        public int CallLogRecepientID { get; set; }
        [ForeignKey("UserLogID")]
        public int CallLogID { get; set; }
        [ForeignKey("Id")]
        public int UserID { get; set; }
        public UserCallLog UserCallLog { get; set; }
        public User User { get; set; }
    }
}
