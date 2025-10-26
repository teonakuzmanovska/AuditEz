namespace App.Domain.Entities.Base;

public interface IBaseDomainEntity<T>
{
    int Id { get; set; }
}