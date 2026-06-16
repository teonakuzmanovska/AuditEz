using System.Diagnostics.CodeAnalysis;

namespace App.Domain.Entities.Audit.Input;

public class AuditLogRequest<T> where T : class
{
    public required ActionInfo ActionInfo { get; set; }
    
    public string EntityId { get; set; }
    
    public T? OldEntity { get; set; }
    
    public T? NewEntity { get; set; }
    
    [SetsRequiredMembers]
    public AuditLogRequest(ActionInfo actionInfo, string entityId, T? oldEntityToLog, T? newEntityToLog)
    {
        ActionInfo = actionInfo;
        EntityId = entityId;
        OldEntity = oldEntityToLog;
        NewEntity = newEntityToLog;
    }
}