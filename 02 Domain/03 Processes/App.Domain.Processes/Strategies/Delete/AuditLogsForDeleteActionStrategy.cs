using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;

namespace App.Domain.Processes.Strategies.Delete;

public class AuditLogsForDeleteActionStrategy : IAuditLogsStrategy
{
    public ActionType ActionType => ActionType.Delete;
    
    public List<AuditLog> Generate<T>(BaseAuditLog baseAuditLog, AuditLogRequest<T> request) where T : class
    {
        ValidateRequest(request);
        
        // TODO: implement audit log generation methods
        
        return new List<AuditLog>();
    }
    
    private void ValidateRequest<T>(AuditLogRequest<T> request) where T : class
    {
        var isDeleteRequestValid = request.Context.Action is ActionType.Delete && request.OldEntity?.Id is not null;

        if (isDeleteRequestValid)
        {
            throw new InvalidAuditRequestException("Invalid request for Delete action.");
        }
    }
}