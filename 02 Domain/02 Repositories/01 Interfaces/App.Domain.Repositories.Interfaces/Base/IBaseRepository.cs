using App.Domain.Entities.Base;

namespace App.Domain.Repositories.Interfaces.Base;

public interface IBaseRepository<TEntity> where TEntity : class, IBaseDomainEntity<int>, new()
{
    /// <summary>
    /// Inserts a new entity in the db.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Insert(TEntity entity);
    
    /// <summary>
    /// Deletes the entity with the provided id from the db.
    /// </summary>
    /// <param name="id"></param>
    void DeleteById(int id);
    
    /// <summary>
    /// Deletes multiple entities by provided ids.
    /// </summary>
    /// <param name="ids"></param>
    void DeleteByIds(IEnumerable<int> ids);

    /// <summary>
    /// Returns the entity that matches the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    TEntity GetById(int id);
    
    /// <summary>
    /// Returns all entities that match the provided ids.
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    List<TEntity> GetByIds(IEnumerable<int> ids);
    
    /// <summary>
    /// Returns all entities from the table
    /// </summary>
    /// <returns></returns>
    List<TEntity> GetAll();
}
