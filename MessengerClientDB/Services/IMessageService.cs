
using MessengerClientDB.Models;

namespace MessengerClientDB.Services
{
    public interface IMessageService
    {
        Messages CreateMessage(string myId, Messages message);
    }
}
