using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class MessageViewModel
    {
        public int MessageThreadID { get; set; }
        public int HouseMemberID { get; set; }
        public String Message { get; set; }
        public String EmailAddress { get; set; }
    }
}
