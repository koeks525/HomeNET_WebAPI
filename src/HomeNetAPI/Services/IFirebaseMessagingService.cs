using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeNetAPI.Services
{
    public interface IFirebaseMessagingService
    {
        Task<bool> SendFirebaseMessage(int id, string title, string message, string keyword, string content, string token, string key);
    }
}
