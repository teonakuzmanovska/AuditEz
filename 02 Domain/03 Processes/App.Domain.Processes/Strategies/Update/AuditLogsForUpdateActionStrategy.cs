using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;

namespace App.Domain.Processes.Strategies.Update;

public class AuditLogsForUpdateActionStrategy : IAuditLogsStrategy
{
    public ActionType ActionType => ActionType.Update;
    
    public List<AuditLog> Generate<T>(BaseAuditLog baseAuditLog, AuditLogRequest<T> request) where T : class
    {
        ValidateRequest(request);
        
        // TODO: implement audit log generation methods

        return new List<AuditLog>();
    }
    
    private void ValidateRequest<T>(AuditLogRequest<T> request) where T : class
    {
        var atLeastOneEntityIsNotNull = request.NewEntity is not null || request.OldEntity is not null;
        var isUpdateRequestValid = request.Context.Action is ActionType.Update && atLeastOneEntityIsNotNull && (request.OldEntity?.Id is not null || request.NewEntity?.Id is not null);
        
        if (isUpdateRequestValid)
        {
            throw new InvalidAuditRequestException("Invalid request for Update action.");
        }
    }
}