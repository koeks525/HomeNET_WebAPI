using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class HouseViewModel
    {
        public int HouseID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String HouseImage { get; set; }
        public String DateCreated { get; set; }
        public int TotalMembers { get; set; }
        public int BannedMembers { get; set; }
        public String Owner { get; set; }
        public int TotalPosts { get; set; }
        public int TotalAnnouncements { get; set; }
        public int TotalComments { get; set; }

    }
}
