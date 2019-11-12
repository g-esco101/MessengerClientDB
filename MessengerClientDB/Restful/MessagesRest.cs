using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;

namespace MessengerClientDB.Restful
{
    public class MessagesRest : IMessagesRest
    {
        // Sends a message to the message service to be stored in its database. 
        public async Task<bool> SendAsync(Messages message)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/message/send")
            {
                Content = new ObjectContent(typeof(Messages), message, new JsonMediaTypeFormatter())
            };
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        // Retrieves messages from the message service. 
        public async Task<List<Messages>> DequeueAsync(string username)
        {
            string myName = HttpUtility.UrlEncode(username);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/message/receive?username={myName}");
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            List<Messages> messages = new List<Messages>();
            if (response.IsSuccessStatusCode)
            {
                messages = await response.Content.ReadAsAsync<List<Messages>>();
            }
            return messages;
        }

        // Deletes the messages from the service database - messages are sent to the user by the specified sender. 
        public async Task<string> DeleteAsync(string username, string sendername)
        {
            string deleteCount = "0";
            string myName = HttpUtility.UrlEncode(username);
            string sender = HttpUtility.UrlEncode(sendername);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete,
                $"/api/message/delete?username={myName}&sendername={sender}");
            var response = await MvcApplication._httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                deleteCount = response.Content.ReadAsStringAsync().Result;
            }
            return deleteCount;
        }
    }
}