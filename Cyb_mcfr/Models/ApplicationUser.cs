using Microsoft.AspNetCore.Identity;

namespace Cyb_mcfr.Models
{
    public class ApplicationUser : IdentityUser
    {
        bool isBlocked = false;
    }
}
