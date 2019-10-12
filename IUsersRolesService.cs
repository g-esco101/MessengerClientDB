using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Services
{
    public interface IUsersRolesService
    {
     //   Dictionary<Users, string> GetAllUsernameRoles(IEnumerable<Users> users);

    //    KeyValuePair<Users, string> GetUsernameRoles(Users user);

        // Generic - var users = from user in _context.Users select user;

        //      string GetUserName(int Id); // Generic
        // Generic (duplicate) - user = _context.Users.Find(Id);

        //     KeyValuePair<Users, string> GetUsernameRoles(int Id);
        // Generic (duplicate) - Users user = _context.Users.Find(Id);

        //    int AddRoles(string username, string[] roleNames);
        //Generic (duplicate) - var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
        //Generic var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
        //_context.UserRolesMapping.Add(map);

        //    int RemoveRoles(string username, string[] roleNames);
        //Generic - var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
        //Generic - var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
        //Generic - var map = _context.UserRolesMapping.Single(a => a.UserID == user.ID && a.RoleID == role.ID);
        //_context.UserRolesMapping.Remove(map);

        //  string[] GetAllRoles();
        //Generic - var allRoles = (from role in _context.RoleMaster orderby role.RoleName select role.RoleName).ToArray();

        // bool IsUserInRole()
        // string[] userRoles = GetUserRoles(username); 

        // string[] GetUserRoles(string username)
        // has big query with many joins.

        //    bool ValidLogin(string username, string providedPwd);
        //Generic - user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        //    bool UserExists(string username);
        //Generic - Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        //    string ReprodcueHash(string username, string password);
        //Generic - Users user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());

        //   bool AddUser(string username, string hash);
        //Generic - _context.Users.Add(user);
    }
}
