using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Services
{
    public interface IFirebaseMessagingService
    {
        Task<bool> SendFirebaseMessage(String title, String message, String token, string key);
    }
}
