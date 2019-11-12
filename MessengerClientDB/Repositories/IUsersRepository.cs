using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IUsersRepository : IRepository<Users>
    {
        Users Get(string username);
        IEnumerable<Users> GetAllUsers();

        Task<Users> GetAsync(string username);
        Task<IEnumerable<Users>> GetAllUsersAsync();
    }
}
