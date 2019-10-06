using MessengerClientDB.Models;
using MessengerClientDB.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MessengerClientDB.Services
{
    public class UsersService : IUsersService
    {
        public UpdateRolesViewModel GetUpdateRolesVMById(KeyValuePair<Users, string> userRoles, string[] allRoles)
        {
            try
            {
                UpdateRolesViewModel updateRoles = new UpdateRolesViewModel();
                updateRoles.Id = userRoles.Key.ID;
                updateRoles.Username = userRoles.Key.Username;
                updateRoles.myRoles = userRoles.Value;
                updateRoles.Roles = allRoles;
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

        public UserRolesViewModel GetUserRolesVMById(KeyValuePair<Users,string> userRoles)
        {
            UserRolesViewModel userRole;
            try
            {
                userRole = new UserRolesViewModel();
                userRole.Id = userRoles.Key.ID;
                userRole.Username = userRoles.Key.Username;
                userRole.myRoles = userRoles.Value;
                return userRole;
            }
            catch { return null; }
        }

        public List<UserRolesViewModel> GetUserRolesVMAll(Dictionary<Users, string> users)
        {
            string userKey, userValue; UserRolesViewModel userRole;
            List<UserRolesViewModel> userRoles = new List<UserRolesViewModel>();
            try
            {
                foreach (var user in users)
                {
                    userKey = user.Key.Username;
                    userValue = user.Value;
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
    }
}