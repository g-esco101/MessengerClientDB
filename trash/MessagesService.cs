using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Services
{
    public class MessagesService : IMessagesService
    {
        public IEnumerable<Messages> FilteredMessages(IEnumerable<Messages> messages, string SearchReceiverID)
        {
            if (!String.IsNullOrEmpty(SearchReceiverID))
            {
                messages = messages.Where(s => s.ReceiverID.Contains(SearchReceiverID)).OrderByDescending(s => s.Date).ThenByDescending(s => s.Time);
            }
            return messages;
        }

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