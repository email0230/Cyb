using Microsoft.AspNetCore.Identity;

namespace Cyb_mcfr.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool isBlocked { get; set; }
        public DateTime PasswordValidity { get; set; }
        public bool NeedToChangePassword { get; set; }
        public string[] PasswordHistory { get; set; } = new string[0];
        public bool EnablePasswordValidation { get; set; } = true;
        //check if there is a better way to make users have this enabled by default
    }

    //public class PasswordHistoryItem
    //{
    //    public int Id { get; set; }
    //    public string UserId { get; set; }
    //    public string PasswordHash { get; set; }
    //    //public DateTime ChangeDate { get; set; }
    //}
}
