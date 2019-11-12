using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IRolesRepository : IRepository<UserRolesMapping>
    {
        IEnumerable<RoleMaster> GetRoleMasters();

        Task<IEnumerable<RoleMaster>> GetRoleMastersAsync();
    }
}
