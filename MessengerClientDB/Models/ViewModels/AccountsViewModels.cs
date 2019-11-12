using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")] //(?=.*[^\a-zA-Z])
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z])(.{8,})$", ErrorMessage = "The Password must contain the following: uppercase letter (A-Z), lowercase (a-z), number (0-9), & special character (e.g. !@#$%^&*)")]
        public string HashedPassword { get; set; }

    }

    public class RegisterVM : LoginVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("HashedPassword", ErrorMessage = "The password must match the confirmation password.")]
        public string ConfirmHashedPassword { get; set; }
    }
}