using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.ViewModels
{
    public class PostDetailsViewModel
    {
        public int HousePostID { get; set; }
        public int TotalLikes { get; set; }
        public int TotalDislikes { get; set; }
        public List<String> Likes { get; set; }
        public List<String> Dislikes { get; set; }
    }
}
