using App.Domain.Config;
using App.Domain.Entities.Audit.Output;
using App.Domain.Repositories.Interfaces;

namespace App.Domain.Repositories.Implementations;

public class AuditLogRepository(IDbConnectionFactory connectionFactory) : IAuditLogRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public Task BulkInsert(List<AuditLog> auditLogs)
    {
        throw new NotImplementedException();
    }
}