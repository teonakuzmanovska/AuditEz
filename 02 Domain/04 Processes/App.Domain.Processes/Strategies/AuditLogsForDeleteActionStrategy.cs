using System.Collections;
using App.Common.Extensions;
using App.Domain.Entities.Attributes;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;

namespace App.Domain.Processes.Strategies;

internal class AuditLogsForDeleteActionStrategy<T> : AuditLogsForActionStrategy<T,DeleteAuditLogRequest<T>> where T : class
{
    protected override BaseAuditLog BuildBaseAuditLog(DeleteAuditLogRequest<T> request)
    {
        return new BaseAuditLog()
        {
            UserId = request.UserId,
            ProcessName = request.ProcessName,
            OriginatingEntityType = typeof(T).Name,
            OriginatingEntityId = request.Entity.GetEntityIdentifier()
        };
    }

    internal override List<AuditLog> Generate(DeleteAuditLogRequest<T> request)
    {
        base.Validate(request);
        
        var baseAuditLog = BuildBaseAuditLog(request);
        
        return GenerateAuditLogsForDelete(baseAuditLog, request.Entity);
    }

    private static List<AuditLog> GenerateAuditLogsForDelete(BaseAuditLog baseAuditLog, object deletedEntity)
    {
        var result = new List<AuditLog>();
        
        var entityType = deletedEntity.GetType();
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            if (propertyInfo.IsDefined(typeof(AuditIgnoreAttribute), inherit: true))
            {
                continue;
            }
            
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(deletedEntity);
            
            if(currentValue is null) continue;

            if (propertyInfo.IsPropertyList())
            {
                var list = currentValue as IList;

                if (list is null || list.Count == 0) continue;
                
                var listItems = list.Cast<object>().ToList();

                if (listItems[0].GetType().IsPrimitive)
                {
                    result.Add(new AuditLog
                    {
                        OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                        EntityId = deletedEntity.GetEntityIdentifier(),
                    
                        UserId = baseAuditLog.UserId,
                        ActionType = ActionType.Delete,
                        ProcessName = baseAuditLog.ProcessName,
                    
                        OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                        EntityType = entityType.Name,
                    
                        PropertyName = propertyInfo.Name,
                        OldPropertyValue = list.TranslateObjectValueToString()
                    });
                }
                else
                {
                    var auditLogsForListProperty = GenerateAuditLogsForListDelete(baseAuditLog, listItems).ToList();
                    result.AddRange(auditLogsForListProperty);
                }
            }
            
            else if (propertyInfo.IsPropertyPrimitiveType())
            {
                var currentValueToString = currentValue.TranslateObjectValueToString();

                result.Add(new AuditLog
                {
                    OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                    EntityId = deletedEntity.GetEntityIdentifier(),

                    UserId = baseAuditLog.UserId,
                    ActionType = ActionType.Delete,
                    ProcessName = baseAuditLog.ProcessName,
                    
                    OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                    EntityType = entityType.Name,
                    
                    PropertyName = propertyInfo.Name,
                    OldPropertyValue = currentValueToString
                });
            }

            else
            {
                var auditLogsForProperty = GenerateAuditLogsForDelete(baseAuditLog, currentValue).ToList();
                
                if (!auditLogsForProperty.Any()) continue;

                result.AddRange(auditLogsForProperty);

            }
        }
        
        return result;
    }

    public static List<AuditLog> GenerateAuditLogsForListDelete(BaseAuditLog baseAuditLog, List<object> listProperty)
    {
        var result = new List<AuditLog>();
        
        if (!listProperty.Any()) return result;

        var listType = listProperty[0].GetType();
        
        foreach (var elementFromListProperty in listProperty)
        {
            foreach (var propertyInfo in listType.GetProperties())
            {
                if (propertyInfo.IsDefined(typeof(AuditIgnoreAttribute), inherit: true))
                {
                    continue;
                }
                
                var currentValue = listType.GetProperty(propertyInfo.Name)?.GetValue(elementFromListProperty);

                if(currentValue is null) continue;

                if (propertyInfo.IsPropertyList())
                {
                    var list = currentValue as IList;

                    if (list is null || list.Count == 0) continue;
                
                    var listItems = list.Cast<object>().ToList();
                
                    var auditLogsForListProperty = GenerateAuditLogsForListDelete(baseAuditLog, listItems).ToList();
                    result.AddRange(auditLogsForListProperty);
                }

                else if (propertyInfo.IsPropertyPrimitiveType())
                {
                    var currentValueToString = currentValue.TranslateObjectValueToString();

                    result.Add(new AuditLog
                    {
                        OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                        EntityId = elementFromListProperty.GetEntityIdentifier(),
                        
                        UserId = baseAuditLog.UserId,
                        ActionType = ActionType.Delete,
                        ProcessName = baseAuditLog.ProcessName,
                        
                        OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                        EntityType = listType.Name,
                        
                        PropertyName = propertyInfo.Name,
                        OldPropertyValue = currentValueToString
                    });
                }

                else
                {
                    var auditLogsForProperty = GenerateAuditLogsForDelete(baseAuditLog, currentValue).ToList();
                
                    if (!auditLogsForProperty.Any()) continue;

                    result.AddRange(auditLogsForProperty);
                }
            }
        }

        return result;
    }
}