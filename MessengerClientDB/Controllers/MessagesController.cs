using MessengerClientDB.Models;
using MessengerClientDB.Restful;
using MessengerClientDB.Services;
using MessengerClientDB.Unity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MessengerClientDB.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class MessagesController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IMessageService _messagesService;

        private IMessagesRest _messageRest;


        public MessagesController(IMessageService messagesService, IMessagesRest messageRest)
        {
            _unitOfWork = new UnitOfWork(new MessengerClient_DBEntities());
            _messagesService = messagesService;
            _messageRest = messageRest;
        }


        public ActionResult Index(string SearchReceiverID)
        {
            string myId = User.Identity.GetUserName();
            if (myId == null)
            {
                return HttpNotFound();
            }
            var messages = _unitOfWork.messagesRepo.GetAllMessagesOrdered(myId);
            if (!string.IsNullOrEmpty(SearchReceiverID))
            {
                messages = _messagesService.FilteredMessages(messages, SearchReceiverID);
            }
            if (messages == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(messages);
        }

        // GET: Messages/Details/5
        public ActionResult Read(int? id)
        {
            Messages message;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            message = _unitOfWork.messagesRepo.Get((int)id);
            if (message == null)
            {
                return HttpNotFound();
            }
            message.Read = "Read";
            _unitOfWork.Save();
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
            reply = _unitOfWork.messagesRepo.Get((int)id);
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
                message = _messagesService.CreateMessage(myID, message);
                bool response = await _messageRest.SendAsync(message);
                if (response)
                {
                    _unitOfWork.messagesRepo.Add(message);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch { }
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
                List<Messages> myMessages = await _messageRest.ArchiveAsync(User.Identity.GetUserName(), received, sent);
                if (myMessages != null)
                {
                    _unitOfWork.messagesRepo.AddRange(myMessages);
                    _unitOfWork.Save();
                }
            }
            catch { }
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
            bool sent = false;
            if (CBReceived)
            {
                received = true;
            }
            if (CBSent)
            {
                sent = true;
            }

            string deleteCount = "0";
            try
            {
                deleteCount = await _messageRest.DeleteAsync(User.Identity.GetUserName(), received, sent);
            }
            catch { }
            ViewBag.DeleteCount = deleteCount + " message(s) deleted";
            return View();
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages message = _unitOfWork.messagesRepo.Get((int)id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedAsync(int id)
        {
            Messages message = _unitOfWork.messagesRepo.Get(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            _unitOfWork.messagesRepo.Remove(message);
            _unitOfWork.Save();
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
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
