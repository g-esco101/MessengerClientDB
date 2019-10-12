using System;
using MessengerClientDB.Repositories;
using MessengerClientDB.Roles;

namespace MessengerClientDB.Unity
{
    public interface IUnitOfWork : IDisposable
    {
        IMessagesRepository messagesRepo { get; }
        IUsersRolesRepository usersRolesRepo { get; }

        void BeginTransaction();
        void CommitTransaction();
        void Rollback();
        bool Save();
    }
}