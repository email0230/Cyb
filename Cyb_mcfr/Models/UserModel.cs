namespace Cyb_mcfr.Models
{
    public class UserModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsBlocked { get; set; } = false;
    }
}
