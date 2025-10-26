using App.Domain.Entities.Base;
using App.Domain.Repositories.Interfaces.Base;

namespace App.Domain.Repositories.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseDomainEntity<int>, new()
{

}
