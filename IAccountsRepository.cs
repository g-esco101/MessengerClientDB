using MessengerClientDB.Models;
using MessengerClientDB.Roles;
using System;
using System.Collections.Generic;

namespace MessengerClientDB.Repositories
{
    public interface IAccountsRepository
    {
        bool ValidLogin(string username, string providedPwd);
        //user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        bool UserExists(string username);
        //Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        string ReprodcueHash(string username, string password);
        //Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        bool AddUser(string username, string hash); // Generic
        //_context.Users.Add(user);
    }
}
