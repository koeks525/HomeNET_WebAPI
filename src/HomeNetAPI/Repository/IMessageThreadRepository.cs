using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IMessageThreadRepository
    {
        List<MessageThread> GetHouseMessages(int houseID);
        MessageThread CreateMessageThread(MessageThread newMessage);
        MessageThread RemoveMessageThread(MessageThread oldThread);
        MessageThread GetMessageThread(int messageThreadID);
        List<MessageThread> GetMessageThreadForMembership(int houseMemberID);
    }
}
