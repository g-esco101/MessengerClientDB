using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IMessagesRepository : IDisposable
    {
        IOrderedQueryable<Messages> GetMessages(string name, string filter);

        Task<Messages> GetReadAsync(int? Id);


        Messages GetReply(int? Id);

        Task<Messages> GetMessageAsync(int? Id);

        bool SaveMessages(List<Messages> myMessages);

        bool SaveMessage(Messages message);

        Task<bool> DeleteAsync(int id);
    }
}
