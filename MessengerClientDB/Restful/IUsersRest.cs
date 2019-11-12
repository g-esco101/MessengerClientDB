using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public interface IUsersRest
    {
        Task<bool> AddRolesServiceAsync(string username, List<string> roles);

        Task<bool> RemoveRolesServiceAsync(string username, List<string> roles);

        Task<bool> DeleteFromService(string username);
    }
}
