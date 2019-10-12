using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Services
{
    public interface IUsersService
    {
        UpdateRolesViewModel GetUpdateRolesVM(Users user, string[] allRoles);

        string stringArrToCSV(string[] stringArr);

        UserRolesViewModel GetUserRolesVM(Users user);

        List<UserRolesViewModel> GetAllUsersRolesVM(IEnumerable<Users> allUsers);
    }
}
