using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public interface IMessagesRest
    {
        Task<bool> SendAsync(Messages message);

        Task<List<Messages>> DequeueAsync(string username);

        Task<string> DeleteAsync(string username, string sendername);

    }
}
