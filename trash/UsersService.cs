using MessengerClientDB.Helpers;
using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Services
{
    public class UsersService : IUsersService
    {
        //   public string[] RoleMasterArray(IEnumerable<RoleMaster> roleMaster)
        //  {
        //    string roleMasterArray = 
        //  }

        public UserRolesViewModel GetUserRolesVM(Users user, IEnumerable<RoleMaster> roleMaster)
        {
            UserRolesViewModel userRole;
            try
            {
                var userRoles = RoleHelper.ArrangeRole(user, roleMaster);
                userRole = new UserRolesViewModel();
                userRole.Id = userRoles.Key.ID;
                userRole.Username = userRoles.Key.Username;
                userRole.MyRoles = userRoles.Value;
                return userRole;
            }
            catch { return null; }
        }

        public UpdateRolesViewModel GetUpdateRolesVM(Users user, IEnumerable<RoleMaster> roleMaster, string[] allRoles)
        {
            try
            {
                Dictionary<string, bool> roles = new Dictionary<string, bool>();
                foreach (var role in allRoles)
                {
                    roles.Add(role, false);
                }
                var userInfo = RoleHelper.ArrangeRole(user, roleMaster);
                UpdateRolesViewModel updateRoles = new UpdateRolesViewModel()
                {
                    Id = userInfo.Key.ID,
                    Username = userInfo.Key.Username,
                    MyRoles = userInfo.Value,
                    Roles = allRoles,
                    Checked = new bool[allRoles.Length]
                };
                return updateRoles;
            }
            catch { return null; }
        }


        public List<UserRolesViewModel> GetUsersRolesVM(IEnumerable<Users> allUsers, IEnumerable<RoleMaster> roleMaster)
        {
            UserRolesViewModel userRole;
            List<UserRolesViewModel> userRoles = new List<UserRolesViewModel>();
            try
            {
                var users = ArrangeRoles(allUsers, roleMaster);
                foreach (var user in users)
                {
                    userRole = new UserRolesViewModel();
                    userRole.Id = user.Key.ID;
                    userRole.Username = user.Key.Username;
                    userRole.MyRoles = user.Value;
                    userRoles.Add(userRole);
                }
                return userRoles;
            }
            catch { return null; }
        }

        private Dictionary<Users, string> ArrangeRoles(IEnumerable<Users> users, IEnumerable<RoleMaster> roleMaster)
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
                        string roleName = roleMaster.Single(r => r.ID == roleMap.RoleMaster.ID).RoleName;
                        myRoles += roleName;
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
    }
}
