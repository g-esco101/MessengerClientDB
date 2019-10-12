using MessengerClientDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        protected MessengerClient_DBEntities _context;

        public UsersRepository(MessengerClient_DBEntities context)
        {
            _context = context;
        }

        public Dictionary<Users, string> GetAllNamesRoles()
        {
            string myRoles = "";
            Dictionary<Users, string> usersRolesDict = new Dictionary<Users, string>();
            var users = from user in _context.Users select user;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
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

        public string GetUserName(int Id)
        {
            string uname = ""; Users user;
            try
            {
                user = _context.Users.Find(Id);  // Returns null if there is no match.
                if (user != null)
                {
                    uname = user.Username;
                }
            }
            catch { }
            return uname;
        }

        public KeyValuePair<Users, string> GetUsernameRoles(int Id)
        {
            string myRoles = ""; KeyValuePair<Users, string> userRoles;
            Users user = _context.Users.Find(Id);  // Returns null if there is no match.
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

        public int AddRoles(string username, string[] roleNames)
        {
            UserRolesMapping map; int rolesAdded = 0;
            string roleName;
            for (int i = 0; i < roleNames.Length; i++)
            {
                map = new UserRolesMapping();
                try
                {
                    roleName = roleNames[i];
                    if (!IsUserInRole(username, roleName))
                    {
                        var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                        var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
                        map.RoleID = role.ID;
                        map.UserID = user.ID;
                        _context.UserRolesMapping.Add(map);
                        rolesAdded++;
                    }
                }
                catch { }
            }
            return rolesAdded;
        }

        public int RemoveRoles(string username, string[] roleNames)
        {
            string roleName; int rolesRemoved = 0;
            for (int i = 0; i < roleNames.Length; i++)
            {
                try
                {
                    roleName = roleNames[i];
                    if (IsUserInRole(username, roleName))
                    {
                        var user = _context.Users.Single(a => a.Username.ToLower() == username.ToLower());
                        var role = _context.RoleMaster.Single(a => a.RoleName == roleName);
                        var map = _context.UserRolesMapping.Single(a => a.UserID == user.ID && a.RoleID == role.ID);
                        _context.UserRolesMapping.Remove(map);
                        rolesRemoved++;
                    }
                }
                catch { }
            }
            return rolesRemoved;
        }

        public string[] GetAllRoles()
        {
            var allRoles = (from role in _context.RoleMaster
                            orderby role.RoleName
                            select role.RoleName).ToArray();
            return allRoles;
        }

        private bool IsUserInRole(string username, string roleName)
        {
            string[] userRoles = GetUserRoles(username);
            foreach (string userRole in userRoles)
            {
                if (userRole == roleName)
                {
                    return true;
                }
            }
            return false;
        }

        private string[] GetUserRoles(string username)
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

        /*
         * 


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
                */
        /*
        public IEnumerable<UserRolesMapping> GetUserRolesMapping(int Id)
        {
            return _context.UserRolesMapping.Where(role => role.UserID == Id).ToList<UserRolesMapping>();
        }

        public IQueryable<UserRolesMapping> GetUserRolesMapping(int Id)
        {
            IQueryable<UserRolesMapping> userRoles = from role in _context.UserRolesMapping
                                                     where role.UserID == Id
                                                     select role;  // Returns an empty IQueryable if there is no match (i.e. it is not null).
            return userRoles;
        }
*/
/*
        public string GetUserRoles(int Id)
                {
                    string myRoles = "";
                    var rolesMaps = _context.UserRolesMapping.Where(roleMap => roleMap.UserID == Id);
                    //         var rolesMaps = from roleMap in _context.UserRolesMapping
                    //                         where roleMap.UserID == Id
                    //                         select roleMap;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
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
*/
        /*
        // Removes user from the Users table & removes the user's UserRolesMappings from the UserRolesMapping table
        public bool DeleteUser(int Id)
        {
            Users singleUser;
            try
            {
                var userRoles = from roles in _context.UserRolesMapping
                                where roles.UserID == Id
                                select roles;   // Returns an empty IQueryable if there is no match (i.e. it is not null).
                foreach (var role in userRoles)
                {
                    _context.UserRolesMapping.Remove(role);
                }
                singleUser = _context.Users.Find(Id);  // Returns null if there is no match.
                _context.Users.Remove(singleUser);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
*/
    }
}