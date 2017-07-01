using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Models
{
    public class SingleResponse<T> where T : class
    {
        public bool DidError { get; set; }
        public string Message { get; set; }     
        public T Model { get; set; }
    }
}
