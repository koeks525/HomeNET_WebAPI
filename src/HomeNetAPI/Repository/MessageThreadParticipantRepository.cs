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

        public List<MessageThreadParticipant> GetMessageParticipants(int messageThreadID)
        {
            var results = dbContext.MessageThreadParticipants.Where(i => i.MessageThreadID == messageThreadID).ToList();
            return results;
        }

        public MessageThreadParticipant RemoveParticipant(int messageThreadParticipantID)
        {
            var selectedParticipant = dbContext.MessageThreadParticipants.First(i => i.MessageThreadParticipantID == messageThreadParticipantID);
            if (selectedParticipant != null)
            {
                dbContext.MessageThreadParticipants.Remove(selectedParticipant);
                dbContext.SaveChanges();
                return selectedParticipant;
            } else
            {
                return null;
            }
        }
    }
}
