using System.Collections;
using App.Common.Extensions;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;

namespace App.Domain.Processes.Strategies;

internal class AuditLogsForCreateActionStrategy<T> : AuditLogsForActionStrategy<T,CreateAuditLogRequest<T>> where T : class
{
    protected override BaseAuditLog BuildBaseAuditLog(CreateAuditLogRequest<T> request)
    {
        return new BaseAuditLog()
        {
            UserId = request.UserId,
            ProcessName = request.ProcessName,
            OriginatingEntityType = typeof(T).Name,
            OriginatingEntityId = request.Entity.GetEntityIdentifier()
        };
    }

    internal override List<AuditLog> Generate(CreateAuditLogRequest<T> request)
    {
        base.Validate(request);
        
        var baseAuditLog = BuildBaseAuditLog(request);
        
        return GenerateAuditLogsForCreate(baseAuditLog, request.Entity);
    }
    
    private List<AuditLog> GenerateAuditLogsForCreate(BaseAuditLog baseAuditLog, object newEntity)
    {
        var result = new List<AuditLog>();
        var entityType = newEntity.GetType();
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(newEntity);

            if(currentValue is null) continue;
            
            if (propertyInfo.IsPropertyList())
            {
                var list = currentValue as IList;

                if (list is null || list.Count == 0) continue;
                
                var listItems = list.Cast<object>().ToList();
                
                var auditLogsForListProperty = GenerateAuditLogsForListCreate(baseAuditLog, listItems).ToList();
                result.AddRange(auditLogsForListProperty);
            }
            
            else if (propertyInfo.IsPropertyPrimitiveType())
            {
                var currentValueToString = currentValue.TranslateObjectValueToString();

                result.Add(new AuditLog
                {
                    OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                    EntityId = newEntity.GetEntityIdentifier(),
                    
                    UserId = baseAuditLog.UserId,
                    ActionType = ActionType.Create,
                    ProcessName = baseAuditLog.ProcessName,
                    
                    OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                    EntityType = newEntity.GetType().Name,
                    
                    PropertyName = propertyInfo.Name,
                    NewPropertyValue = currentValueToString
                });
            }
            
            else
            {
                var auditLogsForProperty = GenerateAuditLogsForCreate(baseAuditLog, currentValue).ToList();

                if (!auditLogsForProperty.Any()) continue;

                result.AddRange(auditLogsForProperty);
            }
        }
        
        return result;
    }

    private List<AuditLog> GenerateAuditLogsForListCreate(BaseAuditLog baseAuditLog, List<object> listProperty)
    {
        var result = new List<AuditLog>();
        
        if (!listProperty.Any()) return result;
        
        var listType = listProperty[0].GetType();

        foreach (var elementFromListProperty in listProperty)
        {
            foreach (var propertyInfo in listType.GetProperties())
            {
                var currentValue = listType.GetProperty(propertyInfo.Name)?.GetValue(elementFromListProperty);
                
                if (currentValue is null) continue;
                
                if (propertyInfo.IsPropertyList())
                {
                    var list = currentValue as IList;

                    if (list is null || list.Count == 0) continue;
                
                    var listItems = list.Cast<object>().ToList();
                
                    var auditLogsForListProperty = GenerateAuditLogsForListCreate(baseAuditLog, listItems).ToList();
                    result.AddRange(auditLogsForListProperty);
                }
                
                else if (propertyInfo.IsPropertyPrimitiveType())
                {
                    var currentValueToString = currentValue.TranslateObjectValueToString();

                    result.Add(new AuditLog
                    {
                        OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                        EntityId = elementFromListProperty.GetEntityIdentifier(),
                        
                        OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                        EntityType = elementFromListProperty.GetType().Name,
                        
                        ActionType = ActionType.Create,
                        ProcessName = baseAuditLog.ProcessName,
                        UserId = baseAuditLog.UserId,
                        
                        PropertyName = propertyInfo.Name,
                        NewPropertyValue = currentValueToString,
                    });
                }
                
                else
                {
                    var auditLogsForProperty = GenerateAuditLogsForCreate(baseAuditLog, currentValue).ToList();

                    if (!auditLogsForProperty.Any()) continue;

                    result.AddRange(auditLogsForProperty);
                }
            }
        }
        
        return result;
    }
}