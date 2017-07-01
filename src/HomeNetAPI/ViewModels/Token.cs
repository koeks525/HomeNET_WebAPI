using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;


namespace HomeNetAPI.ViewModels
{
    public class Token
    {
        public String TokenHandler { get; set; }
        public DateTime Expires { get; set; }
    }
}
