
using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Services
{
    public interface IMessagesService
    {
        IEnumerable<Messages> FilteredMessages(IEnumerable<Messages> messages, string SearchReceiverID);

        Messages CreateMessage(string myId, string receiverID, string contents);
    }
}
