using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;

namespace MessengerClientDB.Restful
{
    public class UsersRest : IUsersRest
    {
        // Rest call to service to add user roles.
        public async Task<bool> AddRolesServiceAsync(string username, List<string> roles)
        {
            string name = HttpUtility.UrlEncode(username);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"/api/users/addroles?username={name}")
            {
                Content = new ObjectContent(typeof(List<string>), roles, new JsonMediaTypeFormatter())
            };
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        // Rest call to service to remove user roles.
        public async Task<bool> RemoveRolesServiceAsync(string username, List<string> roles)
        {
            string name = HttpUtility.UrlEncode(username);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/removeroles?username={name}")
            {
                Content = new ObjectContent(typeof(List<string>), roles, new JsonMediaTypeFormatter())
            };
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        // Rest call to the service to delete user from its database. 
        public async Task<bool> DeleteFromService(string username)
        {
            string name = HttpUtility.UrlEncode(username);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/delete?username={name}");
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}