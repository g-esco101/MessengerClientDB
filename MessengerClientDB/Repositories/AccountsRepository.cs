using System.Linq;
using Unity;
using MessengerClientDB.Models;
using System;

namespace MessengerClientDB.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
   //     [Dependency]
    //    public MessengerClient_DBEntities context { get; set; }

        [Dependency]
        public MessengerClient_DBEntities context = new MessengerClient_DBEntities();


        public bool ValidLogin(string username, string password)
        {
            if (IsNullEmptyWhiteSpace(username, password)) return false;
            Users user;
            try
            {
                user = context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                return Hasher.CheckHash(user.HashedPassword, password);
            }
            catch
            {
                return false;
            }
        }

        public bool UserExists(string username)
        {
            if (IsNullEmptyWhiteSpace(username)) return false;
            try
            {
                Users user = context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetHashSaltIter(string username, string password)
        {
            if (IsNullEmptyWhiteSpace(username, password)) return "";
            string hashSaltIter = "";
            try
            {
                Users user = context.Users.Single(a => a.Username == username);
                hashSaltIter = Hasher.HashGenerator(password);
                return hashSaltIter;
            }
            catch
            {
                return hashSaltIter;
            }
        }

        public string ReprodcueHash(string username, string password)
        {
            if (IsNullEmptyWhiteSpace(username, password)) return "";
            string hashSaltIter = "";
            try
            {
                Users user = context.Users.Single(a => a.Username == username);
                string[] values = user.HashedPassword.Split(':');
                hashSaltIter = Hasher.ReproduceHash(password, values[1], Convert.ToInt32(values[2]));
                return hashSaltIter;
            }
            catch
            {
                return hashSaltIter;
            }
        }

        public bool AddUser(string username, string hash, IUsersRepository usersRepository)
        {
            if (IsNullEmptyWhiteSpace(username, hash)) return false;
            Users user; string role = "User";
            string[] users = new string[1];
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
                context.Users.Add(user);
                context.SaveChanges();
          //      roleProvider = new UsersRepository();
                users[0] = username;
                roles[0] = role;
                usersRepository.AddUsersToRoles(users, roles);
          //      roleProvider.AddUsersToRoles(users, roles);
                return true;
            }
            catch { return false; }
        }

        private bool IsNullEmptyWhiteSpace(string a, string b = "second", string c = "third")
        {
            if ((string.IsNullOrEmpty(a) || string.IsNullOrWhiteSpace(a) || string.IsNullOrEmpty(b) || string.IsNullOrWhiteSpace(b)))
            {
                if ((string.IsNullOrEmpty(c) || string.IsNullOrWhiteSpace(c)))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}