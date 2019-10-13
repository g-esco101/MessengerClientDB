using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MessengerClientDB.Restful
{
    public class AccountRest : IAccountRest
    {
        public async Task<bool> RegisterServiceAsync(string username, string password, string role)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/users/register?username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password) + "&roles=" + role);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        public async Task<bool> GetServiceTokenAsync(string username, string hashedPwd)
        {
            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("username", username));
            formData.Add(new KeyValuePair<string, string>("password", hashedPwd));
            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/token");
                request.Content = new FormUrlEncodedContent(formData);
                var response = await MvcApplication._httpClient.SendAsync(request);
                HttpContent requestContent = response.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(jsonContent);
                string token = json.GetValue("access_token").ToString();
                MvcApplication._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}