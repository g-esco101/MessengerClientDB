using MessengerClientDB.Models;
using System;
using System.Linq;
using System.Web.Security;

namespace MessengerClientDB.Roles
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (MessengerClient_DBEntities _context = new MessengerClient_DBEntities())
            {
                var userRoles = (from user in _context.Users
                                 join roleMapping in _context.UserRolesMapping
                                     on user.ID equals roleMapping.UserID
                                 join role in _context.RoleMaster
                                     on roleMapping.RoleID equals role.ID
                                 where user.Username.ToLower() == username.ToLower()
                                 orderby role.RoleName
                                 select role.RoleName).ToArray();
                return userRoles;
            }
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }
        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}