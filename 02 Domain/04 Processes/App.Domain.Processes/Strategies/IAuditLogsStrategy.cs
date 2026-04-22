using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;

namespace App.Domain.Processes.Strategies;

public interface IAuditLogsStrategy
{
    ActionType ActionType { get; }

    List<AuditLog> Generate<T>(BaseAuditLog baseAuditLog, AuditLogRequest<T> request) where T : class;
}