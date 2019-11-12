using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models.ViewModels
{
    public class MessageVM
    {
        public int ID { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string SenderID { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string ReceiverID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }

        [DataType(DataType.Time)]
        public Nullable<System.TimeSpan> Time { get; set; }

        [StringLength(1000)]
        public string Contents { get; set; }

        [StringLength(6)]
        public string Read { get; set; }

        public Nullable<bool> Queued { get; set; }
    }

    public class MessageReadVM : MessageVM
    {
        [Display(Name = "Change to Read/Unread")]
        public bool IsRead { get; set; }
    }

    public class SendMessageVM
    {
        public int ID { get; set; }
        public string SenderID { get; set; }
        [Display(Name = "To:")]
        public string ReceiverID { get; set; }
        public string Contents { get; set; }
        [Display(Name = "Message")]
        public string MyContents { get; set; }
    }

    public class QueueDeleteMsgVM : MessageVM
    {
        [Display(Name = "Saved in Inbox")]
        public bool Inbox { get; set; }
    }

    public class QueueDeleteVM
    {
        public string Sender { get; set; }
        [Display(Name = "Saved in Inbox")]
        public List<QueueDeleteMsgVM> InInbox { get; set; }

        public QueueDeleteVM()
        {
            InInbox = new List<QueueDeleteMsgVM>();
        }
    }

    internal class MessageComparer : IEqualityComparer<Messages>
    {
        public bool Equals(Messages x, Messages y)
        {
            if (string.Equals(x.ReceiverID, y.ReceiverID) && string.Equals(x.SenderID, y.SenderID) && string.Equals(x.Contents, y.Contents) && string.Equals(x.Date, y.Date) && string.Equals(x.Time, y.Time))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Messages obj)
        {
            string myHashInfo = obj.ReceiverID + obj.SenderID + obj.Contents + obj.Date + obj.Time;
            return myHashInfo.GetHashCode();
        }
    }
}