using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IMessageThreadParticipantRepository
    {
        List<HouseMember> GetMessageParticipants(int messageThreadID);
        MessageThreadParticipant AddParticipant(MessageThreadParticipant newParticipant);
        MessageThreadParticipant RemoveParticipant(int messageThreadParticipantID);
    }
}
