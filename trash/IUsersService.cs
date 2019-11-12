using MessengerClientDB.Models;
using System.Collections.Generic;

namespace MessengerClientDB.Services
{
    public interface IUsersService
    {
        UpdateRolesViewModel GetUpdateRolesVM(Users user, IEnumerable<RoleMaster> roleMaster, string[] allRoles);
        //    UpdateRolesViewModel GetUpdateRolesVM(Users user, string[] allRoles);


     //   string stringArrToCSV(string[] stringArr);
        UserRolesViewModel GetUserRolesVM(Users user, IEnumerable<RoleMaster> roleMaster);

        //     UserRolesViewModel GetUserRolesVM(Users user);

        //    List<UserRolesViewModel> GetUsersRolesVM(IEnumerable<Users> allUsers);

        List<UserRolesViewModel> GetUsersRolesVM(IEnumerable<Users> allUsers, IEnumerable<RoleMaster> roleMaster);

    }
}
