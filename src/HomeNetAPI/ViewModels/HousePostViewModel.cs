using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class HousePostViewModel
    {
        public int HousePostID { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public String PostText { get; set; }
        public int HouseMemberID { get; set; }
        public int IsFlagged { get; set; }
        public String MediaResource { get; set; }
        public String DatePosted { get; set; }
        public int IsDeleted { get; set; }
        public String EmailAddress { get; set; }

    }
}
