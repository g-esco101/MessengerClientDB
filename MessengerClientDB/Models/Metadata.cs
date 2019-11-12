using System;
using System.ComponentModel.DataAnnotations;

namespace MessengerClientDB.Models
{
    public class MessagesMetadata
    {
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string SenderID { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string ReceiverID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Date { get; set; }

        [DataType(DataType.Time)]
        public Nullable<System.TimeSpan> Time { get; set; }

        [StringLength(1000)]
        public string Contents { get; set; }

        [StringLength(6)]
        public string Read { get; set; }

        public Nullable<bool> Queued { get; set; }
    }

    public class UsersMetadata
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z])(.{8,})$", ErrorMessage = "The Password must contain the following: uppercase letter (A-Z), lowercase (a-z), number (0-9), & special character (e.g. !@#$%^&*)")]
        public string HashedPassword { get; set; }
    }
    public class RoleMasterMetadata
    {
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string RoleName { get; set; }
    }

    public class UserRolesMappingMetadata
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int RoleID { get; set; }
    }
}