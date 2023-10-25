using Cyb_mcfr.Data;
using Cyb_mcfr.Interfaces;
using Cyb_mcfr.Models;

namespace Cyb_mcfr.Services
{
    public class ActivityService : IActivityService
    {
        IRepositoryService<Activity> _repository;

        public ActivityService(IRepositoryService<Activity> repository) {
            _repository = repository;
        }

        public void AddAction(Activity a)
        {
            _repository.Add(a);
        }

        public List<Activity> GetAllActivities()
        {
            return _repository.GetAllRecords().ToList();
        }

        public List<Activity> GetActivitiesForUser(string username)
        {
            return _repository.FindBy(x => x.Username == username).ToList();
        }

    }
}
