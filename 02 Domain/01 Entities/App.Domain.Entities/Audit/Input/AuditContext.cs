using App.Domain.Entities.Enum;

namespace App.Domain.Entities.Audit.Input;

public class AuditContext
{
    public required string UserId { get; set; }
    
    public required ActionType Action { get; set; }
    
    public required string ProcessName { get; set; }
    
    public AuditContext(string userId, ActionType action, string processName)
    {
        UserId = userId;
        Action = action;
        ProcessName = processName;
    }
}