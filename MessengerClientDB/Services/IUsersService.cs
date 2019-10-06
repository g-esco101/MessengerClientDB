using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Services
{
    public interface IUsersService
    {
        UpdateRolesViewModel GetUpdateRolesVMById(KeyValuePair<Users, string> userRoles, string[] allRoles);

        string stringArrToCSV(string[] stringArr);

        UserRolesViewModel GetUserRolesVMById(KeyValuePair<Users, string> userRoles);

        List<UserRolesViewModel> GetUserRolesVMAll(Dictionary<Users, string> users);
    }
}
