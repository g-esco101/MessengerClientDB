using MessengerClientDB.Repositories;
using System;
using System.Threading.Tasks;

namespace MessengerClientDB.Unit
{
    public interface IUnitOfWork : IDisposable
    {
        IMessagesRepository messagesRepo { get; }
        IUsersRepository usersRolesRepo { get; }
        IRolesRepository rolesMappingRepo { get; }
        Task<int> SaveAsync();

        //       bool Save();
    }
}