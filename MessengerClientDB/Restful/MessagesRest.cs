using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public class MessagesRest : IMessagesRest
    {
        public async Task<bool> SendAsync(Messages message)
        {
            try
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
            }
            catch { }
            return false;
        }

        public async Task<List<Messages>> ArchiveAsync(string username, bool received, bool sent)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/message/receive?myId=" + username + "&received=" + received + "&sent=" + sent);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<List<Messages>>();
                }
            }
            catch { }
            return null;
        }

        public async Task<string> DeleteAsync(string username, bool received, bool sent)
        {
            string deleteCount = "0";
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/message/delete?myId=" + username + "&received=" + received + "&sent=" + sent);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    deleteCount = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :{0} ", ex.Message);
            }
            return deleteCount;
        }
    }
}