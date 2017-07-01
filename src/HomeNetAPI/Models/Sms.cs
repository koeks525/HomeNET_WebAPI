using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Models
{
    public class Sms
    {
        public string recipientNumber { get; set; }
        public string campaign { get; set; }
        public string dateToSend { get; set; }
        public string dateField { get; set; }
        public string message { get; set; }

    }
}
