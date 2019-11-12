using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public class RolesRepository : Repository<UserRolesMapping>, IRolesRepository
    {
        public RolesRepository(MessengerClient_DBEntities context)
            : base(context)
        {
        }

        public MessengerClient_DBEntities MessengerClient_DBEntities
        {
            get { return Context as MessengerClient_DBEntities; }
        }

        public IEnumerable<RoleMaster> GetRoleMasters()
        {
            return MessengerClient_DBEntities.RoleMaster.OrderBy(x => x.RoleName).ToList();
        }

        public async Task<IEnumerable<RoleMaster>> GetRoleMastersAsync()
        {
            return await MessengerClient_DBEntities.RoleMaster.OrderBy(x => x.RoleName).ToListAsync();
        }
    }
}