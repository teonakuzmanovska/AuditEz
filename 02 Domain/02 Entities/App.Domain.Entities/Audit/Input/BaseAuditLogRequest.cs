using System.Diagnostics.CodeAnalysis;

namespace App.Domain.Entities.Audit.Input;

public abstract class BaseAuditLogRequest<T> where T : class
{
    public required string UserId { get; init; }
    
    public required string ProcessName { get; init; }
    
    [SetsRequiredMembers]
    public BaseAuditLogRequest(string userId, string processName)
    {
        UserId = userId;
        ProcessName = processName;
    }
}