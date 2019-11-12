using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models.ViewModels
{
    public class UserRolesVM
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [Display(Name = "Roles")]
        public string MyRoles { get; set; }
    }

    public class CheckBoxRoles
    {
        public int ID { get; set; }
        public string Role { get; set; }
        public bool IsChecked { get; set; }
    }

    public class RolesUpdateVM : UserRolesVM
    {
        public List<CheckBoxRoles> RoleMasters { get; set; }

        public RolesUpdateVM()
        {
            RoleMasters = new List<CheckBoxRoles>();
        }
    }


}