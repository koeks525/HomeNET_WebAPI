using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.ViewModels
{
    public class NewMessageThreadViewModel
    {
        public int HouseID { get; set; }
        public String ThreadTitle { get; set; }
        public String ThreadMessage { get; set; }
        public String EmailAddress { get; set; }
        public String DateSent { get; set; }
        public List<MessageThreadParticipant> Participants { get; set; }

    }
}
