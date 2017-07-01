using System.ComponentModel.DataAnnotations;

namespace HomeNetAPI.Models
{
    public class LoggedError
    {
        [Key]
        public int LoggedErrorID { get; set; }
        public string ErrorTitle { get; set; }
        public string Description { get; set; }
        public string StackTrace { get; set; }
        
    }
}
