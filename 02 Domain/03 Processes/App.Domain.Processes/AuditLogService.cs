using System.Collections;
using System.Reflection;
using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;

namespace App.Domain.Processes;

public class AuditLogService<T> where T : class
{
    private bool IsAuditLogRequestValid(AuditLogRequest<T> auditLogRequest)
    {
        if (auditLogRequest.Context.Action is ActionType.Unknown) return false;
        
        var atLeastOneEntityIsNotNull = auditLogRequest.NewEntity is not null || auditLogRequest.OldEntity is not null;
        
        var bothEntitiesAreNull = !atLeastOneEntityIsNotNull;
        
        if (bothEntitiesAreNull) return false;

        switch (auditLogRequest.Context.Action)
        {
            case ActionType.Create:
                var isCreateRequestValid = auditLogRequest.Context.Action is ActionType.Create && auditLogRequest.NewEntity is not null;
                return isCreateRequestValid;

            case ActionType.Delete:
                var isDeleteRequestValid = auditLogRequest.Context.Action is ActionType.Delete && auditLogRequest.OldEntity?.Id is not null;
                return isDeleteRequestValid;
            
            case ActionType.Update:
                var isUpdateRequestValid = auditLogRequest.Context.Action is ActionType.Update && atLeastOneEntityIsNotNull && (auditLogRequest.OldEntity?.Id is not null || auditLogRequest.NewEntity?.Id is not null);
                return isUpdateRequestValid;
            
            default:
                return true;
        }
    }
    
    public List<AuditLog> GenerateAuditLogs(AuditLogRequest<T> auditLogRequest)
    {
        if (IsAuditLogRequestValid(auditLogRequest))
        {
            // TODO: add internal message - get from action
            throw new InvalidAuditRequestException("Both old entity and new entity are null.");
        }
        
        var baseAuditData = new BaseAuditLog()
        {
            UserId = auditLogRequest.Context.UserId,
            Action = auditLogRequest.Context.Action,
            ProcessName = auditLogRequest.Context.ProcessName,
            
            EntityType = typeof(T).ToString(),
            EntityId = auditLogRequest.OldEntity?.Id ?? auditLogRequest.NewEntity!.Id
        };

        var auditLogs = new List<AuditLog>();
        
        switch (auditLogRequest.Context.Action)
        {
            case ActionType.Create:
                auditLogs = GenerateAuditLogsForCreate(baseAuditData, auditLogRequest.NewEntity!);
                break;
            case ActionType.Update:
                break;
            case ActionType.Delete:
                break;
        }
        
        return auditLogs;
    }

    private bool IsPropertyList(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsGenericType && typeof(List<>).IsAssignableFrom(propertyInfo.PropertyType.GetGenericTypeDefinition());
    }
    
    private bool IsPropertyPrimitiveType(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string);
    }
    
    private static string TranslatePropertyValueToString(object? value)
    {
        var result = value switch
        {
            // Handle IEnumerable (excluding string and byte)
            IEnumerable enumerable when value is not string and not byte[] =>
                string.Join(",", enumerable.Cast<object>().Select(item => item?.ToString())),

            // Handle everything else
            _ => value?.ToString() ?? string.Empty
        };

        return result;
    }

    private List<AuditLog> GenerateAuditLogsForCreate(BaseAuditLog baseAuditLog, object newEntity)
    {
        var result = new List<AuditLog>();
        var entityType = newEntity.GetType();
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(newEntity);

            if(currentValue is null) continue;
            
            if (IsPropertyList(propertyInfo))
            {
                var list = currentValue as IList;

                if (list is null || list.Count == 0) continue;
                
                var listItems = list.Cast<object>().ToList();
                
                var auditLogsForListProperty = GenerateAuditLogsForListCreate(baseAuditLog, listItems).ToList();
                result.AddRange(auditLogsForListProperty);
            }
            
            else if (IsPropertyPrimitiveType(propertyInfo))
            {
                var currentValueToString = TranslatePropertyValueToString(currentValue);

                result.Add(new AuditLog
                {
                    EntityId = baseAuditLog.EntityId,
                    EntityType = baseAuditLog.EntityType,
                    PropertyName = propertyInfo.Name,
                    NewPropertyValue = currentValueToString,
                    Action = baseAuditLog.Action,
                    ProcessName = baseAuditLog.ProcessName,
                    UserId = baseAuditLog.UserId
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

    private List<AuditLog> GenerateAuditLogsForListCreate(BaseAuditLog baseAuditLog, List<object> newListProperty)
    {
        var result = new List<AuditLog>();
        
        if (!newListProperty.Any()) return result;
        
        var listType = newListProperty[0].GetType();

        foreach (var property in newListProperty)
        {
            foreach (var propertyInfo in listType.GetProperties())
            {
                var currentValue = listType.GetProperty(propertyInfo.Name)?.GetValue(property);
                
                if (currentValue is null) continue;
                
                if (IsPropertyList(propertyInfo))
                {
                    var list = currentValue as IList;

                    if (list is null || list.Count == 0) continue;
                
                    var listItems = list.Cast<object>().ToList();
                
                    var auditLogsForListProperty = GenerateAuditLogsForListCreate(baseAuditLog, listItems).ToList();
                    result.AddRange(auditLogsForListProperty);
                }
                
                else if (IsPropertyPrimitiveType(propertyInfo))
                {
                    var currentValueToString = TranslatePropertyValueToString(currentValue);

                    result.Add(new AuditLog
                    {
                        EntityId = baseAuditLog.EntityId,
                        EntityType = baseAuditLog.EntityType,
                        PropertyName = propertyInfo.Name,
                        NewPropertyValue = currentValueToString,
                        Action = baseAuditLog.Action,
                        ProcessName = baseAuditLog.ProcessName,
                        UserId = baseAuditLog.UserId
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