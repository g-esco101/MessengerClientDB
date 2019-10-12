
using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerClientDB.Services
{
    public interface IMessageService
    {
        IEnumerable<Messages> FilteredMessages(IEnumerable<Messages> messages, string SearchReceiverID);

        Messages CreateMessage(string myId, Messages message);

        /*
                Task<Messages> GetReadAsync(int? Id);

                Messages GetReply(int? Id);

                Task<Messages> GetMessageAsync(int? Id);

                bool AddMessages(List<Messages> myMessages);

                bool AddMessage(Messages message);

                Task<bool> DeleteAsync(int id);
        */
    }
}
