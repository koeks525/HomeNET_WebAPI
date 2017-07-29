using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IMessageThreadMessageRepository
    {
        MessageThreadMessage AddMessageToThread(MessageThreadMessage newMessage);
        List<MessageThreadMessage> GetThreadMessages(int messageThreadID);
    }
}
