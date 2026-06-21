using App.Domain.Entities.Attributes;

namespace App.Domain.Tests.Helpers.TestEntities;

public class TestEmployee
{
    [EntityIdentifier]
    public required Guid UserId { get; init; }
    
    public required  DateTime CreatedOn { get; init; }
    
    public required string Name { get; init; }
    
    public required string Surname { get; init; }
    
    public required string Address { get; init; }
    
    public required string Position { get; init; }
    
    public Department Department { get; init; }
    
    public required double Salary { get; init; }
}

public enum Department
{
    Finance,
    Accounting,
    Marketing
}