using Cyb_mcfr.Controllers;

namespace Cyb_mcfr.Models
{
    public class LayoutViewModel
    {
        public bool IsLoggedIn { get; set; } = false;
        public int SessionDurationMinutes { get; set; } = UsersController.SessionDurationMinutes;
    }
}
