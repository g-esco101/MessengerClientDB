using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Repositories
{
    public interface IUsersRepository
    {
    //Service    Dictionary<Users, string> GetAllNamesRoles(); 
        // var users = from user in _context.Users select user;

        string GetUserName(int Id);
        // user = _context.Users.Find(Id);

        KeyValuePair<Users, string> GetUsernameRoles(int Id);
        // Users user = _context.Users.Find(Id);

        int AddRoles(string username, string[] roleNames);
        //var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
        //var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
        //_context.UserRolesMapping.Add(map);

        int RemoveRoles(string username, string[] roleNames);
        //var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
        //var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
        //var map = _context.UserRolesMapping.Single(a => a.UserID == user.ID && a.RoleID == role.ID);
        //_context.UserRolesMapping.Remove(map);

        string[] GetAllRoles();
        //var allRoles = (from role in _context.RoleMaster orderby role.RoleName select role.RoleName).ToArray();

        // bool IsUserInRole()
        // string[] userRoles = GetUserRoles(username); 

        // string[] GetUserRoles(string username)
        // has big query with many joins.
    }
}
