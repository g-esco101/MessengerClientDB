using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public class UsersRest : IUsersRest
    {
        // Rest call to service to add user roles.
        public async Task<bool> AddRolesServiceAsync(string username, string rolesCSV)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/users/addroles?username=" + username + "&roles=" + rolesCSV);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        // Rest call to service to remove user roles.
        public async Task<bool> RemoveRolesServiceAsync(string username, string rolesCSV)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/users/removeroles?username=" + username + "&roles=" + rolesCSV);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}