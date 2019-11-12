using MessengerClientDB.Models;
using System;

namespace MessengerClient.UnitTest
{
    class Utility
    {
        public static Messages Message(int id, string senderID, string receiverID, string date, string time, string contents, string read)
        {
            Messages m1 = new Messages();
            m1.ID = id;
            m1.SenderID = senderID;
            m1.ReceiverID = receiverID;
            m1.Date = Convert.ToDateTime(date);
            m1.Time = TimeSpan.Parse(time);
            m1.Contents = contents;
            m1.Read = read;
            return m1;
        }
    }
}
