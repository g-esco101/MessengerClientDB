using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string HashedPassword { get; set; }
    }

    public class RegisterViewModel : LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("HashedPassword", ErrorMessage = "The password must match the confirmation password.")]
        public string ConfirmHashedPassword { get; set; }
    }
}