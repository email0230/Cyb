using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Cyb_mcfr.Models
{
    public class EditUserModel
    {
        [Display(Name = "Minimum password length")]
        public int PassMinLength { get; set; } = 14;
        [Display(Name = "Password validity days")]
        public int PassValidityDays { get; set; } = 30;
        [Display(Name = "Password must have digits")]
        public bool PassMustHaveDigits { get; set; } = true;
        public bool PasswordValidation { get; set; } = true;
        [Display(Name = "Bad passwords before lockout")]
        public int PasswordLockoutAttempts { get; set; } = 5;
        [Display(Name = "Minutes before timeout")]
        public int SessionDuration { get; set; } = 5;

        [Required]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string? Password { get; set; }
        public string? NewEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 14)]
        [DataType(DataType.Password)]
        [RegularExpression(@".*\d.*", ErrorMessage = "The Password must contain at least 1 number")]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? NewPasswordConfirm { get; set; }

    }
}
