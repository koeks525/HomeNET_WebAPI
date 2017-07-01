using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class FirebaseMessage
    {
        //Source: https://firebase.google.com/docs/cloud-messaging/http-server-ref#table1 
        public String to { get; set; }
        public Data data { get; set; }
    }
}
