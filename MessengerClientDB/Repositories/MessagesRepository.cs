using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Repositories
{
    public class MessagesRepository : Repository<Messages>, IMessagesRepository
    {
        public MessagesRepository(MessengerClient_DBEntities context)
            : base(context)
        {
        }

        public List<Messages> FilterDuplicates(List<Messages> myMessages)
        {
            List<Messages> filteredMsgs = new List<Messages>();
            try
            {
                foreach (Messages message in myMessages)
                {
                    // Checks if the message already exists
                    var messagesQry = from m in MessengerClient_DBEntities.Messages
                                      where (m.SenderID == message.SenderID & m.ReceiverID == message.ReceiverID
                                      & m.Date == message.Date & m.Time == message.Time & m.Contents == message.Contents)
                                      select m;
                    if (messagesQry.Any())
                    {
                        filteredMsgs.Add(message);
                    }
                }
                return filteredMsgs;
            }
            catch { return filteredMsgs; }
        }

        public IEnumerable<Messages> GetAllMessagesOrdered(string myId)
        {
            return MessengerClient_DBEntities.Messages.Where(m => m.SenderID == myId || m.ReceiverID == myId).
                OrderByDescending(m => m.Date).ThenByDescending(m => m.Time).ToList();
            //        var messages = from m in MessengerClient_DBEntities.Messages
            //                   where m.SenderID == myId || m.ReceiverID == myId
            //                   orderby m.Date descending, m.Time descending
            //                  select m;
            //       return messages.ToList();
        }

        //return PlutoContext.Authors.Include(a => a.Courses).SingleOrDefault(a => a.Id == id);
        //return PlutoContext.Courses.OrderByDescending(c => c.FullPrice).Take(count).ToList();
        //         return PlutoContext.Courses
        ////            .Include(c => c.Author)
        //            .OrderBy(c => c.Name)
        //            .Skip((pageIndex - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToList();

        // Casts the inhereted, generic context to MessengerClient_DBEntities
        public MessengerClient_DBEntities MessengerClient_DBEntities
        {
            get { return Context as MessengerClient_DBEntities; }
        }
    }
}