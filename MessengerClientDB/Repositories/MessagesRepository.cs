using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public class MessagesRepository : Repository<Messages>, IMessagesRepository
    {
        public MessagesRepository(MessengerClient_DBEntities context)
            : base(context)
        {
        }

        public MessengerClient_DBEntities MessengerClient_DBEntities
        {
            get { return Context as MessengerClient_DBEntities; }
        }

        public IEnumerable<Messages> GetInbox(string myName)
        {
            return MessengerClient_DBEntities.Messages.Where(m => m.ReceiverID == myName && m.Queued == true).
                OrderByDescending(m => m.Date).ThenByDescending(m => m.Time).ToList();
        }

        public IEnumerable<Messages> GetSent(string myName)
        {
            return MessengerClient_DBEntities.Messages.Where(m => m.SenderID == myName && m.Queued == false).
                OrderByDescending(m => m.Date).ThenByDescending(m => m.Time).ToList();
        }

        public async Task<IEnumerable<Messages>> GetInboxAsync(string myName)
        {
            return await MessengerClient_DBEntities.Messages.Where(m => m.ReceiverID == myName && m.Queued == true).
                OrderByDescending(m => m.Date).ThenByDescending(m => m.Time).ToListAsync();
        }

        public async Task<IEnumerable<Messages>> GetSentAsync(string myName)
        {
            return await MessengerClient_DBEntities.Messages.Where(m => m.SenderID == myName && m.Queued == false).
                OrderByDescending(m => m.Date).ThenByDescending(m => m.Time).ToListAsync();
        }

        public void AddRange(IEnumerable<Messages> myMessages)
        {
            List<Messages> uniqueMsgs = new List<Messages>();
            foreach (Messages message in myMessages)
            {
                // Checks if the message already exists
                var messagesQry = from m in MessengerClient_DBEntities.Messages
                                  where (m.SenderID == message.SenderID && m.ReceiverID == message.ReceiverID
                                  && m.Date == message.Date && m.Time == message.Time && m.Contents == message.Contents
                                  && m.Queued == true)
                                  select m;
                if (!messagesQry.Any())
                {
                    uniqueMsgs.Add(message);
                }
            }
            base.AddRange(uniqueMsgs);
        }
    }
}
