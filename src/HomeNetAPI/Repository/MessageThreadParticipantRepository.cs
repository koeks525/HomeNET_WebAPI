using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class MessageThreadParticipantRepository : IMessageThreadRepository
    {
        private HomeNetContext dbContext;

        public MessageThreadParticipantRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public MessageThread CreateMessageThread(MessageThread newMessage)
        {
            var thread = dbContext.MessageThreads.Add(newMessage);
            dbContext.SaveChanges();
            if (thread != null)
            {
                return thread.Entity;
            } else
            {
                return null;
            }
        }

        public List<MessageThread> GetHouseMessages(int houseID)
        {
            var house = dbContext.Houses.First(i => i.HouseID == houseID);
            if (house == null)
            {
                return null;
            }
            var houseMembers = dbContext.MessageThreads.Where(i => i.HouseMemberID == house.HouseID);
        }

        public MessageThread RemoveMessageThread(MessageThread oldThread)
        {
            throw new NotImplementedException();
        }
    }
}
