using App.Domain.Entities.Audit.Output;

namespace App.Domain.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task BulkInsert(List<AuditLog> auditLogs);
}