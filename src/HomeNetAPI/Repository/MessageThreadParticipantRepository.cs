using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class MessageThreadParticipantRepository : IMessageThreadParticipantRepository
    {
        private HomeNetContext dbContext;

        public MessageThreadParticipantRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public MessageThreadParticipant AddParticipant(MessageThreadParticipant newParticipant)
        {
            throw new NotImplementedException();
        }

        public MessageThread CreateMessageThread(MessageThread newMessage)
        {
            throw new NotImplementedException();
        }

        public List<MessageThread> GetHouseMessages(int houseID)
        {
            throw new NotImplementedException();
        }

        public List<HouseMember> GetMessageParticipants(int messageThreadID)
        {
            throw new NotImplementedException();
        }

        public MessageThread RemoveMessageThread(MessageThread oldThread)
        {
            throw new NotImplementedException();
        }

        public MessageThreadParticipant RemoveParticipant(int messageThreadParticipantID)
        {
            throw new NotImplementedException();
        }
    }
}
