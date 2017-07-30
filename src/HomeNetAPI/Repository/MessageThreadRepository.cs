using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;


namespace HomeNetAPI.Repository
{
    public class MessageThreadRepository : IMessageThreadRepository
    {
        private HomeNetContext dbContext;

        public MessageThreadRepository(HomeNetContext dbContext)
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
            }
            else
            {
                return null;
            }
        }

        public List<MessageThread> GetHouseMessages(int houseMemberID)
        {
            var result = dbContext.MessageThreads.Where(i => i.HouseMemberID == houseMemberID).ToList();
            return result;
        }

        public MessageThread RemoveMessageThread(MessageThread oldThread)
        {
            var result = dbContext.MessageThreads.First(i => i.MessageThreadID == oldThread.MessageThreadID);
            if (result != null)
            {
                dbContext.MessageThreads.Remove(result);
                dbContext.SaveChanges();
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
