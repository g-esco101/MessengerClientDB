using System.Linq;
using MessengerClientDB.Helpers;
using MessengerClientDB.Models;
using System;
using MessengerClientDB.Roles;

namespace MessengerClientDB.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        protected MessengerClient_DBEntities _context;

        public AccountsRepository(MessengerClient_DBEntities context)
        {
            _context = context;
        }

        public bool ValidLogin(string username, string password)
        {
            if (Checker.IsNullEmptyWhiteSpace(username, password)) return false;
            Users user;
            try
            {
                user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                return Hasher.CheckHash(user.HashedPassword, password);
            }
            catch
            {
                return false;
            }
        }

        public bool UserExists(string username)
        {
            if (Checker.IsNullEmptyWhiteSpace(username)) return false;
            try
            {
                Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ReprodcueHash(string username, string password)
        {
            if (Checker.IsNullEmptyWhiteSpace(username, password)) return "";
            string hashSaltIter = "";
            try
            {
                Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                string[] values = user.HashedPassword.Split(':');
                hashSaltIter = Hasher.ReproduceHash(password, values[1], Convert.ToInt32(values[2]));
                return hashSaltIter;
            }
            catch
            {
                return hashSaltIter;
            }
        }

        public bool AddUser(string username, string hash)
        {
            if (Checker.IsNullEmptyWhiteSpace(username, hash)) return false;
            Users user;
            string[] roles = new string[1];
            try
            {
                if (UserExists(username))
                {
                    return false;
                }
                user = new Users();
                user.Username = username;
                user.HashedPassword = hash;
                _context.Users.Add(user);
                return true;
            }
            catch { return false; }
        }
    }
}