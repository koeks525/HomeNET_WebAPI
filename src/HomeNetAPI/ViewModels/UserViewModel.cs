using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.ViewModels
{
    public class UserViewModel
    {
        [Required, Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }
        public string EmailAddress { get; set; }
    }
}
