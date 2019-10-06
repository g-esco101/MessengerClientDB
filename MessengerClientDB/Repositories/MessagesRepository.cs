﻿using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace MessengerClientDB.Repositories
{
    // Contains all of the data access code for the application. 
    public class MessagesRepository : IMessagesRepository
    {
        [Dependency]
        public MessengerClient_DBEntities context = new MessengerClient_DBEntities();

        public IOrderedQueryable<Messages> GetMessages(string myId, string SearchReceiverID)
        {
            IOrderedQueryable<Messages> messages = null;
            try
            {
                messages = from m in context.Messages
                           where m.SenderID == myId || m.ReceiverID == myId
                           orderby m.Date descending, m.Time descending
                           select m;


     /*           messages = from m in context.Messages
                           where m.SenderID == myId || m.ReceiverID == myId
                           orderby m.Date descending
                           orderby m.Time descending
                           select m;
*/
            }
            catch { return null; }
            if (!String.IsNullOrEmpty(SearchReceiverID))
            {
                try
                {
                    messages = messages.Where(s => s.ReceiverID.Contains(SearchReceiverID)).OrderByDescending(s => s.Date).ThenByDescending(s => s.Time);
                }
                catch { return null; }
            }
            return messages;
        }

        public async Task<Messages> GetReadAsync(int? Id)
        {
            Messages message = await context.Messages.FindAsync(Id);
            if (message == null)
            {
                return null;
            }
            message.Read = "Read";
            context.SaveChanges();
            return message;
        }


        public Messages GetReply(int? Id)
        {
            try
            {
                Messages message = new Messages();
                message.ReceiverID = context.Messages.Find(Id).ReceiverID; // Possibly looks for ReceiveID on null.
                return message;
            }
            catch { return null; }
        }

        // Gets a message from the database.
        public async Task<Messages> GetMessageAsync(int? Id)
        {
            Messages message = await context.Messages.FindAsync(Id); // Returns null if there is no match.
            return message;
        }

        public bool SaveMessage(Messages message)
        {
            try
            {
                context.Messages.Add(message);
                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        // Saves messages to the database. It does not save messages that already exist.
        public bool SaveMessages(List<Messages> myMessages)
        {
            try
            {
                foreach (Messages message in myMessages)
                {
                    // Checks if the message already exists
                    var messagesQry = from m in context.Messages
                                      where (m.SenderID == message.SenderID & m.ReceiverID == message.ReceiverID
                                      & m.Date == message.Date & m.Time == message.Time & m.Contents == message.Contents)
                                      select m;
                    if (messagesQry.Count() == 0)
                    {
                        context.Messages.Add(message);
                    }
                }
                context.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        // Deletes a message from the Users table
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                Messages message = await context.Messages.FindAsync(id);
                context.Messages.Remove(message);
                await context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        // Disposes of resources.
        public void Dispose()
        {
            context.Dispose();
        }
    }
}