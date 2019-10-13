using System.Threading.Tasks;

namespace MessengerClientDB.Restful
{
    public interface IAccountRest
    {
        Task<bool> RegisterServiceAsync(string username, string password, string role);

        Task<bool> GetServiceTokenAsync(string username, string hashedPwd);
    }
}
