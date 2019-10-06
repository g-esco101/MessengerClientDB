using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace MessengerClientDB.Repositories
{
    public class UsersRepository : RoleProvider, IUsersRepository
    {
        protected MessengerClient_DBEntities context = new MessengerClient_DBEntities();

        public IQueryable<UserRolesMapping> GetUserRolesMapping(int Id)
        {
            IQueryable<UserRolesMapping> userRoles = from role in context.UserRolesMapping
                                                     where role.UserID == Id
                                                     select role;  // Returns an empty IQueryable if there is no match (i.e. it is not null).
            return userRoles;
        }

        public Dictionary<Users, string> GetAllUsersRoles()
        {
            string myRoles = "";
            Dictionary<Users, string> usersRolesDict = new Dictionary<Users, string>();
            IQueryable<Users> users = from user in context.Users select user;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
            try
            {
                foreach (Users user in users)
                {
                    int i = 0; int roleMapLength = user.UserRolesMapping.Count();
                    foreach (UserRolesMapping roleMap in user.UserRolesMapping)
                    {
                        myRoles += roleMap.RoleMaster.RoleName;
                        if ((i + 1) < roleMapLength)
                        {
                            myRoles += ", ";
                        }
                        i++;
                    }
                    usersRolesDict.Add(user, myRoles);
                    myRoles = "";
                }
                return usersRolesDict;
            }
            catch { return null; }
        }

        public KeyValuePair<Users, string> GetUsersRoles(int Id)
        {
            string myRoles = ""; KeyValuePair<Users, string> userRoles;
            Users user = context.Users.Find(Id);  // Returns null if there is no match.
            try
            {
                int i = 0; int roleMapLength = user.UserRolesMapping.Count();
                foreach (UserRolesMapping roleMap in user.UserRolesMapping)
                {
                    myRoles += roleMap.RoleMaster.RoleName;
                    if ((i + 1) < roleMapLength)
                    {
                        myRoles += ", ";
                    }
                    i++;
                }
                userRoles = new KeyValuePair<Users, string>(user, myRoles);
                return userRoles;
            }
            catch { return new KeyValuePair<Users, string>(null, ""); }
        }

        public string GetUserRoles(int Id)
        {
            string myRoles = "";
            var rolesMaps = from roleMap in context.UserRolesMapping
                            where roleMap.UserID == Id
                            select roleMap;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
            try
            {
                int i = 0; int roleMapLength = rolesMaps.Count();
                foreach (var role in rolesMaps)
                {
                    myRoles += role.RoleMaster.RoleName;
                    if ((i + 1) < roleMapLength)
                    {
                        myRoles += ", ";
                    }
                    i++;
                }                
                return myRoles;
            }
            catch { return ""; }
        }

        public string GetUserName(int Id)
        {
            string uname = ""; Users user;
            try
            {
                user = context.Users.Find(Id);  // Returns null if there is no match.
                if (user != null)
                {
                    uname = user.Username;
                }
            }
            catch { }
            return uname;
        }

        // Removes user from the Users table & removes the user's UserRolesMappings from the UserRolesMapping table
        public bool DeleteUser(int Id)
        {
            Users singleUser;
            try
            {
                var userRoles = from roles in context.UserRolesMapping
                                where roles.UserID == Id
                                select roles;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
                foreach (var role in userRoles)
                {
                    context.UserRolesMapping.Remove(role);
                }
                singleUser = context.Users.Find(Id);  // Returns null if there is no match.
                context.Users.Remove(singleUser);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }

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

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            UserRolesMapping map;
            string username, roleName;
            //        for (int i = 0; i < usernames.Length; i++)
            for (int i = 0; i < roleNames.Length; i++)
            {
                map = new UserRolesMapping();
                try
                {
                    username = usernames[i];
                    roleName = roleNames[i];
                    if (!IsUserInRole(username, roleName))
                    {
                        var user = context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                        var role = context.RoleMaster.Single(a => a.RoleName == roleName);
                        map.RoleID = role.ID;
                        map.UserID = user.ID;
                        context.UserRolesMapping.Add(map);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                }
            }
            context.SaveChanges();
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
        public override string[] GetAllRoles()
        {
            var allRoles = (from role in context.RoleMaster
                            orderby role.RoleName
                            select role.RoleName).ToArray();
            return allRoles;
        }

        public override string[] GetRolesForUser(string username)
        {
            var userRoles = (from user in context.Users
                             join roleMapping in context.UserRolesMapping
                             on user.ID equals roleMapping.UserID
                             join role in context.RoleMaster
                             on roleMapping.RoleID equals role.ID
                             where user.Username.ToLower() == username.ToLower()
                             orderby role.RoleName
                             select role.RoleName).ToArray();
            return userRoles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] userRoles = GetRolesForUser(username);
            foreach (string userRole in userRoles)
            {
                if (userRole == roleName)
                {
                    return true;
                }
            }
            return false;
        }
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            string username, roleName;
            for (int i = 0; i < roleNames.Length; i++)
            {
                try
                {
                    username = usernames[i];
                    roleName = roleNames[i];
                    if (IsUserInRole(username, roleName))
                    {
                        var user = context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                        var role = context.RoleMaster.Single(a => a.RoleName == roleName);
                        var map = context.UserRolesMapping.Single(a => a.UserID == user.ID && a.RoleID == role.ID);
                        context.UserRolesMapping.Remove(map);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                }
            }
            context.SaveChanges();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public void RemoveRoles(string username, string[] roleName)
        {
            string[] usernames = new string[roleName.Length];
            for (int i = 0; i < roleName.Length; i++)
            {
                usernames[i] = username;
            }
            RemoveUsersFromRoles(usernames, roleName);
        }

        public void AddRoles(string username, string[] roleName)
        {
            string[] usernames = new string[roleName.Length];
            for (int i = 0; i < roleName.Length; i++)
            {
                usernames[i] = username;
            }
            AddUsersToRoles(usernames, roleName);
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
    }
}