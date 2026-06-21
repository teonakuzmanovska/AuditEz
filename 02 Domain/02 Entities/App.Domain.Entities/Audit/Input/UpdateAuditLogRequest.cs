using System.Diagnostics.CodeAnalysis;

namespace App.Domain.Entities.Audit.Input;

public class UpdateAuditLogRequest<T> : BaseAuditLogRequest<T>
    where T : class
{
    public required T OldEntity { get; init; }
    
    public required T NewEntity { get; init; }

    [SetsRequiredMembers]
    public UpdateAuditLogRequest(string userId, string processName, T newEntityToLog, T oldEntityToLog) : base(userId, processName)
    {
        NewEntity = newEntityToLog;
        OldEntity = oldEntityToLog;
    }
}