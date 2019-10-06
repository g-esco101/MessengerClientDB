using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models
{
    public class UserRolesViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [Display(Name = "Roles")]
        public string myRoles { get; set; }
    }

    public class UpdateRolesViewModel : UserRolesViewModel
    {
        public string[] Roles { get; set; } //Roles to update
        public bool[] Checked { get; set; } // if checked, correspoding role in Roles will be updated.

    }
}