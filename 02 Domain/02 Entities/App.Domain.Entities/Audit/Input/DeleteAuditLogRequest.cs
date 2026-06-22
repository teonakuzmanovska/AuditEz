using System.Diagnostics.CodeAnalysis;

namespace App.Domain.Entities.Audit.Input;

public class DeleteAuditLogRequest<T> : BaseAuditLogRequest<T>
    where T : class
{
    public T Entity { get; set; }

    [SetsRequiredMembers]
    public DeleteAuditLogRequest(string userId, string processName, T entityToLog) : base(userId, processName)
    {
        Entity = entityToLog;
    }
}