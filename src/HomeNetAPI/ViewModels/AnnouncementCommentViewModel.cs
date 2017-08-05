using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class AnnouncementCommentViewModel
    {
        public String Name { get; set; }
        public String Surname { get; set; }
        public String EmailAddress { get; set; }
        public int AnnouncementCommentID { get; set; }
        public int HouseMemberID { get; set; }
        public string Comment { get; set; }
        public string DateAdded { get; set; }
        public int IsDeleted { get; set; }
        public int IsFlagged { get; set; }
        public int HouseAnnouncementID { get; set; }

    }
}
