using App.Domain.Entities.Base;

namespace App.Domain.Repositories.Interfaces.Base;

public interface IBaseRepository<TEntity> where TEntity : class, IBaseDomainEntity<int>, new()
{

}
