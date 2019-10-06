using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClientDB.Repositories
{
    public interface IUsersRepository : IDisposable
    {
        IQueryable<UserRolesMapping> GetUserRolesMapping(int Id);

        Dictionary<Users, string> GetAllUsersRoles();

        KeyValuePair<Users, string> GetUsersRoles(int Id);

        string GetUserRoles(int Id);

        string GetUserName(int Id);

        bool DeleteUser(int Id);

        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

        bool IsUserInRole(string username, string roleName);

        string[] GetRolesForUser(string username);

        string[] GetAllRoles();

        void AddUsersToRoles(string[] usernames, string[] roleNames);

        void RemoveRoles(string username, string[] roleName);

        void AddRoles(string username, string[] roleName);
    }
}
