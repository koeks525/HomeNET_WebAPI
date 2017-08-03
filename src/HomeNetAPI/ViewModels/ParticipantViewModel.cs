using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class ParticipantViewModel
    {
        public int UserID { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public String EmailAddress { get; set; }
        public int MessageThreadParticipantID { get; set; }
        public int IsDeleted { get; set; }
        public int MessageThreadID { get; set; }
        public int HouseMemberID { get; set; }

    }
}
