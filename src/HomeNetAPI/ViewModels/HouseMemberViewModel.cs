using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class HouseMemberViewModel
    {
        public int HouseMemberID { get; set; }
        public int UserID { get; set; }
        public int CountryID { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public String EmailAddress { get; set; }
        public String Reason { get; set; }
        
    }
}
