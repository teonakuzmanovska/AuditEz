using App.Domain.Entities.Enum;

namespace App.Domain.Entities.Audit.Output;

public class BaseAuditLog
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public required string UserId { get; set; }
    
    public required string ProcessName { get; set; }
    
    public required string OriginatingEntityType { get; set; }
    
    public required string OriginatingEntityId { get; set; }
}