using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class MessagesViewModel
    {
        public int MessageThreadMessageID { get; set; }
        public String Message { get; set; }
        public String DateSent { get; set; }
        public int MessageThreadID { get; set; }
        public int HouseMemberID { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public String EmailAddress { get; set; }
    }
}
