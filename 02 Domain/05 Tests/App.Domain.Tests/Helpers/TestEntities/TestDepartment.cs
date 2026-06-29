using App.Domain.Entities.Attributes;

namespace App.Domain.Tests.Helpers.TestEntities;

public class TestDepartment
{
    [EntityIdentifier]
    public required Guid Id { get; init; }
    
    [AuditIgnore]
    public required  DateTime CreatedOn { get; init; }
    
    public required Department Department { get; init; }
    
    public required List<TestEmployee> Employees { get; init; } = new();
    
    public required List<int> YearsActive { get; init; } = new();
}