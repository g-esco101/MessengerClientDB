using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Services
{
    public class UsersRolesService : IUsersRolesService
    {
        public Dictionary<Users, string> GetAllUsernameRoles(IEnumerable<Users> users)
        {
            string myRoles = "";
            Dictionary<Users, string> usersRolesDict = new Dictionary<Users, string>();
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

        public KeyValuePair<Users, string> GetUsernameRoles(Users user)
        {
            string myRoles = ""; KeyValuePair<Users, string> userRoles;
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

        /*
         * 


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
*/
    }
}