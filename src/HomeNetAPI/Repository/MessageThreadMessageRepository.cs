using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class MessageThreadMessageRepository : IMessageThreadMessageRepository
    {
        private HomeNetContext dbContext;

        public MessageThreadMessageRepository(HomeNetContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public MessageThreadMessage AddMessageToThread(MessageThreadMessage newMessage)
        {
            var result = dbContext.MessageThreadMessages.Add(newMessage);
            dbContext.SaveChanges();
            return result.Entity;

        }

        public List<MessageThreadMessage> GetThreadMessages(int messageThreadID)
        {
            var result = dbContext.MessageThreadMessages.Where(i => i.MessageThreadID == messageThreadID).ToList();
            return result;
        }
    }
}
