using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using MessengerClientDB.Models;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using System.Net.Http.Headers;
using System.Web;
using System.Net.Http.Formatting;
using System.Linq;
using System.Collections.Generic;
using MessengerClientDB.Repositories;
using MessengerClientDB.Services;

namespace MessengerClientDB.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class MessagesController : Controller
    {
        private IMessagesRepository _messagesRepo;

        private IMessageService _messagesService;

        public MessagesController(IMessagesRepository msgsRepository, IMessageService msgsService)
        {
            this._messagesRepo = msgsRepository;
            this._messagesService = msgsService;
        }

        public ActionResult Index(string SearchReceiverID)
        {
            string myId = User.Identity.GetUserName();
            if (myId == null)
            {
                return HttpNotFound();
             //   return View();
            }
            var messages = _messagesRepo.GetMessages(myId, SearchReceiverID);
            if (messages == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(messages);
        }

        // GET: Messages/Details/5
        public async Task<ActionResult> Read(int? id)
        {
            Messages message;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            message = await _messagesRepo.GetReadAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Send
        public ActionResult Send(int? id)
        {
            Messages reply;
            if (id == null)
            {
                return View();
            }
            reply = _messagesRepo.GetReply(id);
            return View(reply);
        }

        // POST: Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Send([Bind(Include = "ID,ReceiverID,SenderID,Date,Time,Contents,Read")] Messages message)
        {
            if (!ModelState.IsValid)
            {
                return View(message);
            }
            try
            {
                string myID = User.Identity.GetUserName();
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/message/send");

                message = _messagesService.CreateMessage(myID, message);
                requestMessage.Content = new ObjectContent(typeof(Messages), message, new JsonMediaTypeFormatter());
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        _messagesRepo.SaveMessage(message);
                        return RedirectToAction("Index");
                    }
                    catch (HttpRequestException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception :{0} ", ex.Message);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :{0} ", ex.Message);
            }
            return View(message);
        }

        // GET: Messages/Archive/5
        public ActionResult Archive()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Archive(bool CBReceived = false, bool CBSent = false)
        {
            bool received = false;
            bool sent = false;
            if (CBReceived)
            {
                received = true;
            }
            if (CBSent)
            {
                sent = true;
            }
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/message/receive?myId=" + User.Identity.GetUserName() + "&received=" + received + "&sent=" + sent);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);

       //         var response = await MvcApplication._httpClient.GetAsync("/api/message/receive?myId=" + User.Identity.GetUserName() + "&receivedMsgs=" + received + "&sentMsgs=" + sent);
                List<Messages> myMessages = null;
                if (response.IsSuccessStatusCode)
                {
                    myMessages = await response.Content.ReadAsAsync<List<Messages>>();
                    _messagesRepo.SaveMessages(myMessages);
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :{0} ", ex.Message);
            }
            return RedirectToAction("Index");
        }

        // GET: Messages/Delete/5
        public ActionResult ArchiveDelete()
        {
            return View();
        }
        // POST: Messages/GetSent
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> ArchiveDelete(bool CBReceived = false, bool CBSent = false)
        {
            bool received = false;
            bool sent = false; ;
            string JsonContent = "0";
            if (CBReceived)
            {
                received = true;
            }
            if (CBSent)
            {
                sent = true;
            }
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/message/delete?myId=" + User.Identity.GetUserName() + "&received=" + received + "&sent=" + sent);
                var response = await MvcApplication._httpClient.SendAsync(requestMessage);

       //         var response = await MvcApplication._httpClient.DeleteAsync("/api/message/delete?myId=" + User.Identity.GetUserName() + "&received=" + received + "&sent=" + sent);
                if (response.IsSuccessStatusCode)
                {
                    JsonContent = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception :{0} ", ex.Message);
            }
            ViewBag.DeleteCount = JsonContent + " message(s) deleted";
            return View();
        }

        // GET: Messages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages message = await _messagesRepo.GetMessageAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(int id)
        {
            await _messagesRepo.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public HttpStatusCodeResult Validator(Messages m)
        {

            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, modelErrors);
            }
            return null;
        }

        // Disposes of resources.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _messagesRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
