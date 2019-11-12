using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Helpers
{
    public static class RoleHelper
    {
        public static KeyValuePair<Users, string> ArrangeRole(Users user, IEnumerable<RoleMaster> roleMaster)
        {
            string myRoles = "";
            KeyValuePair<Users, string> userRoles;
            try
            {
                int i = 0;
                int roleMapLength = user.UserRolesMapping.Count();
                foreach (UserRolesMapping roleMap in user.UserRolesMapping)
                {
                    string roleName = roleMaster.Single(r => r.ID == roleMap.RoleMaster.ID).RoleName;
                    myRoles += roleName;
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
    }
}