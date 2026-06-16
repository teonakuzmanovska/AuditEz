using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;
using App.Domain.Processes.Strategies;

namespace App.Domain.Processes;

public class AuditLogService<T> where T : class
{
    private Dictionary<ActionType, IAuditLogsStrategy> _strategies => new()
        {
            { ActionType.Create, new AuditLogsForCreateActionStrategy() },
            { ActionType.Update, new AuditLogsForUpdateActionStrategy() },
            { ActionType.Delete, new AuditLogsForDeleteActionStrategy() },
        };
    
    public List<AuditLog> GenerateAuditLogs(AuditLogRequest<T> request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        if (request.ActionInfo.Action is ActionType.Unknown)
        {
            throw new InvalidAuditRequestException("Action is a required field.");
        }
        
        var baseAuditLog = BuildBaseAuditLog(request);
        var strategy = _strategies[request.ActionInfo.Action];
        
        return strategy.Generate(baseAuditLog, request);
    }

    private BaseAuditLog BuildBaseAuditLog(AuditLogRequest<T> request)
    {
        if (request.EntityId is null)
        {
            throw new InvalidAuditRequestException("Entity Id must be provided for auditing.");
        }

        return new BaseAuditLog()
        {
            UserId = request.ActionInfo.UserId,
            ActionType = request.ActionInfo.Action,
            ProcessName = request.ActionInfo.ProcessName,
            EntityType = typeof(T).Name,
            EntityId = request.EntityId
        };
    }
}
