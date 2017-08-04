using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class NewAnnouncementViewModel
    {
        public String Title { get; set; }
        public String Message { get; set; }
        public int HouseID { get; set; }
        public String EmailAddress { get; set; }
    }
}
