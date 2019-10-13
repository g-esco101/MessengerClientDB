using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public interface IUsersRest
    {
        Task<bool> AddRolesServiceAsync(string username, string rolesCSV);

        Task<bool> RemoveRolesServiceAsync(string username, string rolesCSV);
    }
}
