using MessengerClientDB.Models;
using System.Linq;

namespace MessengerClientDB.Repositories
{
    public class UsersRolesRepository : Repository<Users>, IUsersRolesRepository
    {
        public UsersRolesRepository(MessengerClient_DBEntities context)
            : base(context)
        {
        }

        public MessengerClient_DBEntities MessengerClient_DBEntities
        {
            get { return Context as MessengerClient_DBEntities; }
        }

        public int AddRoles(string username, string[] roleNames)
        {
            UserRolesMapping map; string roleName; int rolesAdded = 0;
            for (int i = 0; i < roleNames.Length; i++)
            {
                try
                {
                    var user = MessengerClient_DBEntities.Users.Single(a => a.Username.ToLower() == username.ToLower());
                    roleName = roleNames[i];
                    if (!IsUserInRole(user, roleName))
                    {
                        var role = MessengerClient_DBEntities.RoleMaster.Single(a => a.RoleName == roleName);
                        map = new UserRolesMapping()
                        {
                            UserID = user.ID,
                            RoleID = role.ID
                        };
                        MessengerClient_DBEntities.UserRolesMapping.Add(map);
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
            try
            {
                var user = MessengerClient_DBEntities.Users.Single(a => a.Username.ToLower() == username.ToLower());
                for (int i = 0; i < roleNames.Length; i++)
                {
                    roleName = roleNames[i];
                    if (IsUserInRole(user, roleName))
                    {
                        var role = MessengerClient_DBEntities.RoleMaster.Single(a => a.RoleName == roleName);
                        var map = MessengerClient_DBEntities.UserRolesMapping.Single(a => a.UserID == user.ID && a.RoleID == role.ID);
                        MessengerClient_DBEntities.UserRolesMapping.Remove(map);
                        rolesRemoved++;
                    }
                }
            }
            catch { }
            return rolesRemoved;
        }

        private bool IsUserInRole(Users user, string roleName)
        {
            try
            {
                var roleMaster = MessengerClient_DBEntities.RoleMaster.Single(r => r.RoleName == roleName);
                var roleMapping = MessengerClient_DBEntities.UserRolesMapping.Where(r => r.UserID == user.ID && r.RoleID == roleMaster.ID);
                return roleMapping.Count() == 1;
            }
            catch { return false; }
        }

        public string[] GetAllRoles()
        {
            return MessengerClient_DBEntities.RoleMaster.Select(x => x.RoleName).ToArray();
        }
    }
}