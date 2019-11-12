using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public class UsersRepository : Repository<Users>, IUsersRepository
    {
        public UsersRepository(MessengerClient_DBEntities context)
            : base(context)
        {
        }

        public MessengerClient_DBEntities MessengerClient_DBEntities
        {
            get { return Context as MessengerClient_DBEntities; }
        }

        public Users Get(string username)
        {
            return MessengerClient_DBEntities.Users.SingleOrDefault(u => u.Username.ToLower() == username.ToLower());
        }

        public IEnumerable<Users> GetAllUsers()
        {
            return MessengerClient_DBEntities.Users.Include("UserRolesMapping").ToList();
        }

        public async Task<Users> GetAsync(string username)
        {
            return await MessengerClient_DBEntities.Users.SingleOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await MessengerClient_DBEntities.Users.Include("UserRolesMapping").ToListAsync();
        }
    }
}
