using App.Domain.Entities.Enum;

namespace App.Domain.Entities.Audit.Output;

public class AuditLog : BaseAuditLog
{
    public string? Id { get; private set; }
    
    public required ActionType ActionType { get; set; }
    
    public required string EntityType { get; set; }
    
    public required string EntityId { get; set; }
    
    public required string PropertyName { get; set; }
    
    public string? OldPropertyValue { get; set; }
    
    public string? NewPropertyValue { get; set; }
}