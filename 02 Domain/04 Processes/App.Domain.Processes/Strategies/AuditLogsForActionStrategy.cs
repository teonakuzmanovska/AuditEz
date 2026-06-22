using App.Domain.Entities.Attributes;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;

namespace App.Domain.Processes.Strategies;

internal abstract class AuditLogsForActionStrategy<T,R> where T : class where R : BaseAuditLogRequest<T>
{
    protected virtual void Validate(R request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var identifierProperties = typeof(T)
            .GetProperties()
            .Where(p => Attribute.IsDefined(
                p,
                typeof(EntityIdentifierAttribute)))
            .ToList();

        if (identifierProperties.Count != 1)
        {
            throw new InvalidOperationException(
                $"Exactly one EntityIdentifierAttribute is required for the {nameof(T)} class.");
        }
    }

    protected abstract BaseAuditLog BuildBaseAuditLog(R request);
    
    internal abstract List<AuditLog> Generate(R request);
}