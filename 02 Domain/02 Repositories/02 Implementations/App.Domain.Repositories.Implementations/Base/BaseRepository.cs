using App.Domain.Entities.Base;
using App.Domain.Repositories.Interfaces.Base;

namespace App.Domain.Repositories.Implementations.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseDomainEntity<int>, new()
{
    public int Insert(T entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteById(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public T GetById(int id)
    {
        throw new NotImplementedException();
    }

    public List<T> GetByIds(IEnumerable<int> ids)
    {
        throw new NotImplementedException();
    }

    public List<T> GetAll()
    {
        throw new NotImplementedException();
    }

}
