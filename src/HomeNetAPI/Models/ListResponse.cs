using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Models
{
    public class ListResponse<T> where T:class
    {
        public string Message { get; set; }
        public bool DidError { get; set; }
        public List<T> Model { get; set; }
    }
}
