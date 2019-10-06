using MessengerClientDB.Models;
using System;
using System.Collections.Generic;

namespace MessengerClientDB.Repositories
{
    public interface IAccountsRepository : IDisposable
    {
        bool ValidLogin(string username, string providedPwd);

        bool UserExists(string username);

        string GetHashSaltIter(string username, string password);

        string ReprodcueHash(string username, string password);

        bool AddUser(string username, string hash, IUsersRepository usersRepository);    }
}
