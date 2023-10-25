using Cyb_mcfr.Models;
using NuGet.Protocol.Core.Types;

namespace Cyb_mcfr.Interfaces
{
    public interface IActivityService
    {
        public void AddAction(Activity a);

        public List<Activity> GetAllActivities();

        public List<Activity> GetActivitiesForUser(string username);
    }
}
