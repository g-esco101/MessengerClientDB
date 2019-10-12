using MessengerClientDB.Helpers;
using MessengerClientDB.Models;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Services
{
    public class UsersService : IUsersService
    {
        public UpdateRolesViewModel GetUpdateRolesVM(Users user, string[] allRoles)
        {
            try
            {
                var userInfo = RoleHelper.ArrangeRole(user);
                UpdateRolesViewModel updateRoles = new UpdateRolesViewModel()
                {
                    Id = userInfo.Key.ID,
                    Username = userInfo.Key.Username,
                    myRoles = userInfo.Value,
                    Roles = allRoles,
                };
                return updateRoles;
            }
            catch { return null; }
        }

        public string stringArrToCSV(string[] stringArr)
        {
            string myRoles = "";
            for (int i = 0; i < stringArr.Length; i++)
            {
                myRoles += stringArr[i];
                if ((i + 1) < stringArr.Length)
                {
                    myRoles += ", ";
                }
            }
            return myRoles;
        }

        public List<UserRolesViewModel> GetAllUsersRolesVM(IEnumerable<Users> allUsers)
        {
            UserRolesViewModel userRole;
            List<UserRolesViewModel> userRoles = new List<UserRolesViewModel>();
            try
            {
                var users = ArrangeRoles(allUsers);
                foreach (var user in users)
                {
                    userRole = new UserRolesViewModel();
                    userRole.Id = user.Key.ID;
                    userRole.Username = user.Key.Username;
                    userRole.myRoles = user.Value;
                    userRoles.Add(userRole);
                }
                return userRoles;
            }
            catch { return null; }
        }

        public UserRolesViewModel GetUserRolesVM(Users user)
        {
            UserRolesViewModel userRole;
            try
            {
                var userRoles = RoleHelper.ArrangeRole(user);
                userRole = new UserRolesViewModel();
                userRole.Id = userRoles.Key.ID;
                userRole.Username = userRoles.Key.Username;
                userRole.myRoles = userRoles.Value;
                return userRole;
            }
            catch { return null; }
        }

        private Dictionary<Users, string> ArrangeRoles(IEnumerable<Users> users)
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
    }
}