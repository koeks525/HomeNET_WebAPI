using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Services
{
    public interface IMailMessage
    {
        Boolean SendMailMessage(String destinationName, String destinationEmail, String title, String message);
    }
}
