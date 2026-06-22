using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Processes.Exceptions;
using App.Domain.Processes.Strategies;

namespace App.Domain.Processes;

public static class AuditLogService
{
    public static List<AuditLog> GenerateAuditLogs<T>(BaseAuditLogRequest<T> request) where T : class
    {
        var auditLogs = request switch
        {
            CreateAuditLogRequest<T> createRequest => new AuditLogsForCreateActionStrategy<T>().Generate(createRequest),

            UpdateAuditLogRequest<T> updateRequest => new AuditLogsForUpdateActionStrategy<T>().Generate(updateRequest),

            DeleteAuditLogRequest<T> deleteRequest => new AuditLogsForDeleteActionStrategy<T>().Generate(deleteRequest),

            _ => throw new InvalidAuditRequestException("Unsupported request.")
        };
        
        return auditLogs;
    }
}
