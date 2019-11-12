using MessengerClientDB.Models;
using MessengerClientDB.Repositories;
using System.Threading.Tasks;

namespace MessengerClientDB.Unit
{
    public class UnitOfWork : IUnitOfWork
    {
        private MessengerClient_DBEntities _context;


        public UnitOfWork()
        {
            _context = new MessengerClient_DBEntities();
            messagesRepo = new MessagesRepository(_context);
            usersRolesRepo = new UsersRepository(_context);
            rolesMappingRepo = new RolesRepository(_context);
        }

        public IMessagesRepository messagesRepo { get; private set; }
        public IUsersRepository usersRolesRepo { get; private set; }
        public IRolesRepository rolesMappingRepo { get; private set; }

        //     public bool Save()
        //     {
        //         try
        //         {
        //            _context.SaveChanges();
        //          return true;
        //      }
        //       catch { return false; }
        //  }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch { return -1; }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}