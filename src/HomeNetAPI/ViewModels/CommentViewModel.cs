using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class CommentViewModel
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String EmailAddress { get; set; }
        public int HousePostID { get; set; }
        public int HousePostCommentID { get; set; }
        public String Comment { get; set; }
        public String DateAdded { get; set; }
        public int HouseMemberID { get; set; }

    }
}
