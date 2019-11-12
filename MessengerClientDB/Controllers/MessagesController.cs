using MessengerClientDB.Models;
using MessengerClientDB.Models.ViewModels;
using MessengerClientDB.Restful;
using MessengerClientDB.Unit;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MessengerClientDB.Controllers
{
    public class MessagesController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IMessagesRest _messageRest;


        public MessagesController(IMessagesRest messageRest)

        {
            _unitOfWork = new UnitOfWork();
            _messageRest = messageRest;
        }

        // Displays the received messages. 
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Inbox(string SearchReceiverID)
        {
            string myName = User.Identity.GetUserName();
            var messages = await _unitOfWork.messagesRepo.GetInboxAsync(myName);
            if (!string.IsNullOrEmpty(SearchReceiverID))
            {
                messages = messages.Where(s => s.SenderID.Contains(SearchReceiverID)).OrderByDescending(s => s.Date).ThenByDescending(s => s.Time);
            }
            List<MessageVM> messagesVM = new List<MessageVM>();
            foreach (Messages msg in messages)
            {
                MessageVM msgVM = CreateMsgVM(msg);
                msgVM.ID = msg.ID;
                messagesVM.Add(msgVM);
            }
            return View(messagesVM);
        }

        // Displays all the sent messages. 
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Sent(string SearchReceiverID)
        {
            string myName = User.Identity.GetUserName();
            var messages = await _unitOfWork.messagesRepo.GetSentAsync(myName);
            if (!string.IsNullOrEmpty(SearchReceiverID))
            {
                messages = messages.Where(s => s.ReceiverID.Contains(SearchReceiverID)).OrderByDescending(s => s.Date).ThenByDescending(s => s.Time);
            }
            List<MessageVM> messagesVM = new List<MessageVM>();
            foreach (Messages msg in messages)
            {
                MessageVM msgVM = CreateMsgVM(msg);
                msgVM.ID = msg.ID;
                messagesVM.Add(msgVM);
            }
            return View(messagesVM);
        }

        // Displays a message.
        // GET: Messages/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Read(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages message = await _unitOfWork.messagesRepo.GetAsync((int)id);
            if (message == null)
            {
                return HttpNotFound();
            }
            MessageReadVM model = new MessageReadVM
            {
                ID = message.ID,
                SenderID = message.SenderID,
                ReceiverID = message.ReceiverID,
                Date = message.Date,
                Time = message.Time,
                Contents = message.Contents,
                Read = message.Read,
                Queued = message.Queued,
                IsRead = false
            };
            return View(model);
        }

        // Saves the changes made to a messages read value. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Read([Bind(Include = "ID,IsRead")] MessageReadVM model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Messages message = await _unitOfWork.messagesRepo.GetAsync(model.ID);
            if (message == null)
            {
                return HttpNotFound();
            }
            if (model.IsRead)
            {
                message.Read = "Read";
            }
            else
            {
                message.Read = "Unread";
            }
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Inbox");
        }

        // Gets a message that is to be saved on the service's database & that is to be stored on the client's database. 

        // Get: Messages/SendMessage
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> SendMessage(int? id)
        {
            SendMessageVM reply = new SendMessageVM();
            string myName = User.Identity.GetUserName();
            if (id == null)
            {
                reply.Contents = null;
                return View(reply);
            }
            Messages message = await _unitOfWork.messagesRepo.GetAsync((int)id);
            if (message == null)
            {
                return HttpNotFound();
            }
            reply.ReceiverID = message.ReceiverID == myName ? message.SenderID : message.ReceiverID;
            reply.ID = message.ID;
            reply.Contents = message.Contents;
            return View(reply);
        }

        // Sends a message to be saved on the service's database & also stores the message on the client's database. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> SendMessage([Bind(Include = "ID,ReceiverID,MyContents")] SendMessageVM message)
        {
            if (!ModelState.IsValid)
            {
                return View(message);
            }
            Users user = await _unitOfWork.usersRolesRepo.GetAsync(message.ReceiverID);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User does not exist.");
                return View(message);
            }
            string myName = User.Identity.GetUserName();
            Messages myMessage = CreateMessage(myName, message.ReceiverID, message.MyContents);
            bool response = await _messageRest.SendAsync(myMessage);
            if (response)
            {
                _unitOfWork.messagesRepo.Add(myMessage);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Inbox");
            }
            return View(myMessage);
        }

        // Gets & displays messages that are stored on the service's database. 
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Dequeue(int? id)
        {
            List<Messages> messages = await _messageRest.DequeueAsync(User.Identity.GetUserName());
            List<MessageVM> messagesVM = new List<MessageVM>();
            foreach (Messages msg in messages)
            {
                messagesVM.Add(CreateMsgVM(msg));
            }
            TempData["messagesVM"] = messagesVM;
            return View(messagesVM);
        }

        // Saves messages from the service's database on the client's database. 
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Dequeue()
        {
            if (TempData["messagesVM"] == null)
            {
                return RedirectToAction("Inbox");
            }
            List<MessageVM> messagesVM = (List<MessageVM>)TempData["messagesVM"];
            List<Messages> messages = new List<Messages>();
            foreach (var message in messagesVM)
            {
                messages.Add(new Messages
                {
                    SenderID = message.SenderID,
                    ReceiverID = message.ReceiverID,
                    Date = message.Date,
                    Time = message.Time,
                    Contents = message.Contents,
                    Read = message.Read,
                    Queued = message.Queued
                });
            }
            _unitOfWork.messagesRepo.AddRange(messages);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Inbox");
        }

        // Get method to delete messages stored on the service database. 
        // GET: Messages/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> QueueDelete()
        {
            string myName = User.Identity.GetUserName();
            List<Messages> msgsQueued = await _messageRest.DequeueAsync(myName);
            var inbox = await _unitOfWork.messagesRepo.GetInboxAsync(myName);
            QueueDeleteVM queueVM = new QueueDeleteVM();
            foreach (Messages msg in msgsQueued)
            {
                QueueDeleteMsgVM msgVM = CreateQueueDeleteMsgVM(msg, inbox);
                queueVM.InInbox.Add(msgVM);
            }
            return View(queueVM);
        }

        // Deletes messages stored on the service database. 
        // POST: Messages/GetSent
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> QueueDelete([Bind(Include = "Sender")]QueueDeleteVM model)
        {
            Users user = await _unitOfWork.usersRolesRepo.GetAsync(model.Sender);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User does not exist.");
                return View(model);
            }
            List<MessageVM> msgsDelete = new List<MessageVM>();
            string sender = model.Sender;
            string myName = User.Identity.GetUserName();
            string deleteCount = "0";
            if (!String.IsNullOrEmpty(sender) && !String.IsNullOrWhiteSpace(sender))
            {
                deleteCount = await _messageRest.DeleteAsync(myName, sender);
            }
            ViewBag.DeleteCount = deleteCount + " message(s) deleted";
            List<Messages> msgsQueued = await _messageRest.DequeueAsync(myName);
            QueueDeleteVM queueVM = new QueueDeleteVM();
            var inbox = await _unitOfWork.messagesRepo.GetInboxAsync(myName);
            foreach (Messages msg in msgsQueued)
            {
                QueueDeleteMsgVM msgVM = CreateQueueDeleteMsgVM(msg, inbox);
                queueVM.InInbox.Add(msgVM);
            }
            return View(queueVM);
        }

        // Get method to delete a message from the client database. 
        // GET: Messages/Delete/5
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages message = await _unitOfWork.messagesRepo.GetAsync((int)id);
            MessageVM messageVM = CreateMsgVM(message);
            messageVM.ID = message.ID;
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(messageVM);
        }

        // Deletes a message from the client database.
        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> DeleteConfirmedAsync(int id)
        {
            Messages message = await _unitOfWork.messagesRepo.GetAsync(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            _unitOfWork.messagesRepo.Remove(message);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Inbox");
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

        // Creates a view model for message. 
        private MessageVM CreateMsgVM(Messages message)
        {
            return (new MessageVM
            {
                SenderID = message.SenderID,
                ReceiverID = message.ReceiverID,
                Date = message.Date,
                Time = message.Time,
                Contents = message.Contents,
                Read = message.Read,
                Queued = message.Queued
            });
        }

        // Creates a view model for QueueDelete.
        private QueueDeleteMsgVM CreateQueueDeleteMsgVM(Messages message, IEnumerable<Messages> inbox)
        {
            MessageComparer msgComparer = new MessageComparer();
            return (new QueueDeleteMsgVM
            {
                SenderID = message.SenderID,
                ReceiverID = message.ReceiverID,
                Date = message.Date,
                Time = message.Time,
                Contents = message.Contents,
                Read = message.Read,
                Queued = message.Queued,
                Inbox = inbox.Contains(message, msgComparer)
            });
        }

        // Creates a message.
        public Messages CreateMessage(string myId, string receiverID, string contents)
        {
            Messages message = new Messages();
            DateTime myDateTime = DateTime.Now;
            string sqlFormattedTime = myDateTime.ToString("hh:mm:ss tt");
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");
            TimeSpan timespan = DateTime.Parse(sqlFormattedTime).TimeOfDay;
            DateTime myDate = DateTime.Parse(sqlFormattedDate);
            message.Date = myDate;
            message.Time = timespan;
            message.SenderID = myId;
            message.ReceiverID = receiverID;
            message.Contents = contents;
            message.Read = "Unread";
            message.Queued = false;
            return message;
        }
    }
}
