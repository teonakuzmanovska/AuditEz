using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;
using App.Domain.Processes.Strategies;

namespace App.Domain.Processes;

public class AuditLogService<T> where T : class
{
    private readonly Dictionary<ActionType, IAuditLogsStrategy> _strategies;

    public AuditLogService(List<IAuditLogsStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.ActionType);
    }
    
    public List<AuditLog> GenerateAuditLogs(AuditLogRequest<T> request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (!_strategies.TryGetValue(request.Context.Action, out var strategy))
            throw new InvalidAuditRequestException(
                $"No audit strategy registered for action {request.Context.Action}.");

        var baseAuditLog = BuildBaseAuditLog(request);

        return strategy.Generate(baseAuditLog, request);
    }

    private BaseAuditLog BuildBaseAuditLog(AuditLogRequest<T> request)
    {
        var entityId = request.OldEntity?.Id ?? request.NewEntity?.Id;

        if (entityId is null)
        {
            throw new InvalidAuditRequestException("Entity Id must be provided for auditing.");
        }

        return new BaseAuditLog()
        {
            UserId = request.Context.UserId,
            ActionType = request.Context.Action,
            ProcessName = request.Context.ProcessName,
            EntityType = typeof(T).Name,
            EntityId = entityId
        };
    }
}
