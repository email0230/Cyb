using Cyb_mcfr.Extensions;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Cyb_mcfr.Interfaces
{
    public interface IRepositoryService<T> where T : IEntity
    {
        IQueryable<T> GetAllRecords();

        T GetSingle(int id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        ServiceResult Add(T entity);
        ServiceResult Delete(T entity);
        ServiceResult Edit(T entity);
        ServiceResult Save(T entity);
    }
}
