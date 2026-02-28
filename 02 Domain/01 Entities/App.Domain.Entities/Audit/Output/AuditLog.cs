using App.Domain.Entities.Audit.Base;

namespace App.Domain.Entities.Audit.Output;

public class AuditLog : BaseAuditLog
{
    public string Id { get; set; }
    
    public required string PropertyName { get; set; }
    
    public string? OldPropertyValue { get; set; }
    
    public string? NewPropertyValue { get; set; }

    public AuditLog() : base() { }
}