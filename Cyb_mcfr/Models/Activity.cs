using Cyb_mcfr.Interfaces;

namespace Cyb_mcfr.Models
{
    public class Activity : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
    }
}
