using App.Domain.Entities.Enum;

namespace App.Domain.Entities.Audit.Base;

public class BaseAuditLog
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public required string UserId { get; set; }
    
    public required ActionType Action { get; set; }
    
    public required string ProcessName { get; set; }
    
    public required string EntityId { get; set; }
    
    public required string EntityType { get; set; }
}