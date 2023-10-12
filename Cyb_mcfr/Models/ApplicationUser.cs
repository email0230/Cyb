using Microsoft.AspNetCore.Identity;

namespace Cyb_mcfr.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool isBlocked { get; set; }
        public DateTime PasswordValidity { get; set; }
    }
}
