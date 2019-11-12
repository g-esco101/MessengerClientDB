using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IMessagesRepository : IRepository<Messages>
    {
        IEnumerable<Messages> GetInbox(string myName);
        IEnumerable<Messages> GetSent(string myName);

        Task<IEnumerable<Messages>> GetInboxAsync(string myName);
        Task<IEnumerable<Messages>> GetSentAsync(string myName);

        void AddRange(IEnumerable<Messages> myMessages);
    }
}
