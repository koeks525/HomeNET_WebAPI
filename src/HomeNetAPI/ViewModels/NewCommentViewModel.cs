using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class NewCommentViewModel
    {
        public int HouseAnnouncementID { get; set; }
        public String Comment { get; set; }
        public String EmailAddress { get; set; }
    }
}
