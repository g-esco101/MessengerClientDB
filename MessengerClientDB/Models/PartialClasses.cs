using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models
{
    [MetadataType(typeof(MessagesMetadata))]
    public partial class Messages
    {
    }

   [MetadataType(typeof(UsersMetadata))]
    public partial class Users
    {
    }

    [MetadataType(typeof(RoleMasterMetadata))]
    public partial class RoleMaster
    {
    }

    [MetadataType(typeof(UserRolesMappingMetadata))]
    public partial class UserRolesMapping
    {
    }
}