using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IMessagesRepository : IRepository<Messages>
    {
        IEnumerable<Messages> GetAllMessagesOrdered(string myId);

        List<Messages> FilterDuplicates(List<Messages> myMessages);

    //*** May implement     IOrderedQueryable<Messages> GetMessages(string myId, string SearchReceiverID);
    //      messages = from m in _context.Messages
    //        where m.SenderID == myId || m.ReceiverID == myId
    //        orderby m.Date descending, m.Time descending
    //                       select m;

        //    Task<Messages> GetReadAsync(int? Id);
        //Generic - Messages message = await _context.Messages.FindAsync(Id);
        //*** Doesn't belong - message.Read = "Read";
        //    _context.SaveChanges();

        //     Messages GetReply(int? Id);
        //*** Generic - message.ReceiverID = _context.Messages.Find(Id).ReceiverID; 

        //  Task<Messages> GetMessageAsync(int? Id); // Generic
        //Generic - Messages message = await _context.Messages.FindAsync(Id);

        //    bool AddMessages(List<Messages> myMessages); // Generic
        //Generic - _context.Messages.Add(message);

        //      bool AddMessage(Messages message)// Generic 
        //Generic -       var messagesQry = from m in _context.Messages
        //                       where (m.SenderID == message.SenderID & m.ReceiverID == message.ReceiverID
        //                       & m.Date == message.Date & m.Time == message.Time & m.Contents == message.Contents)
        //                       select m;
        //Generic - _context.Messages.Add(message);

        //      Task<bool> DeleteAsync(int id); // Generic
        //Generic -    Messages message = await _context.Messages.FindAsync(id);
        // Generic    _context.Messages.Remove(message);
        //     await _context.SaveChangesAsync();

    }
}
