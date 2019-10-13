using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public interface IMessagesRest
    {
        Task<bool> SendAsync(Messages message);

        Task<List<Messages>> ArchiveAsync(string username, bool received, bool sent);

        Task<string> DeleteAsync(string username, bool received, bool sent);

    }
}
