using MessengerClientDB.Models;
using MessengerClientDB.Repositories;
using System.Data.Entity;

namespace MessengerClientDB.Unity
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MessengerClient_DBEntities _context;
        private DbContextTransaction _transaction;

        public UnitOfWork(MessengerClient_DBEntities context)
        {
            _context = context;
            messagesRepo = new MessagesRepository(_context);
            usersRolesRepo = new UsersRolesRepository(_context);
        }

        public IMessagesRepository messagesRepo { get; private set; }
        public IUsersRolesRepository usersRolesRepo { get; private set; }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public bool Save()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}