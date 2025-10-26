namespace App.Domain.Entities.Base;

public class BaseDomainEntity : IBaseDomainEntity<int>
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }
}
