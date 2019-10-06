using MessengerClientDB.Models;
using System;

namespace MessengerClientDB.Services
{
    public class MessageService : IMessageService
    {

        public Messages CreateMessage(string myId, Messages message)
        {
            try
            {
                DateTime myDateTime = DateTime.Now;
                string sqlFormattedTime = myDateTime.ToString("hh:mm:ss tt");
                string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");
                TimeSpan timespan = DateTime.Parse(sqlFormattedTime).TimeOfDay;
                DateTime myDate = DateTime.Parse(sqlFormattedDate);
                message.Date = myDate;
                message.Time = timespan;
                message.SenderID = myId;
                message.Read = "Unread";
                return message;
            }
            catch { return null; }
        }
    }
}