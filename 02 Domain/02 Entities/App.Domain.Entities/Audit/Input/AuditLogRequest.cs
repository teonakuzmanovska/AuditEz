namespace App.Domain.Entities.Audit.Input;

public class AuditLogRequest<T> where T : class
{
    public required AuditContext Context { get; set; }
    
    public EntityToLog<T>? OldEntity { get; set; }
    
    public EntityToLog<T>? NewEntity { get; set; }

    public AuditLogRequest() { }

    public AuditLogRequest(AuditContext context, EntityToLog<T>? oldEntity, EntityToLog<T>? newEntity)
    {
        Context = context;
        OldEntity = oldEntity;
        NewEntity = newEntity;
    }
}