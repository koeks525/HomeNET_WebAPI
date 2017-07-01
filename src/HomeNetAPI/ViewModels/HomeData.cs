using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class HomeData
    {
        public int TotalPosts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBannedUsers { get; set; }
        public int TotalPendingUsers { get; set; }
        public int TotalActiveUsers { get; set; }
        public String DateCreated { get; set; }

    }
}
