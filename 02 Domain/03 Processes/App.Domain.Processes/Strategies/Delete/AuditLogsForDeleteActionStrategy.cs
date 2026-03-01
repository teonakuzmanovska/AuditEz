using System.Collections;
using App.Common.Extensions;
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
        
        return GenerateAuditLogsForDelete(baseAuditLog, request.NewEntity!);
    }
    
    private void ValidateRequest<T>(AuditLogRequest<T> request) where T : class
    {
        var isDeleteRequestValid = request.Context.Action is ActionType.Delete && request.OldEntity?.Id is not null;

        if (isDeleteRequestValid)
        {
            throw new InvalidAuditRequestException("Invalid request for Delete action.");
        }
    }

    private static List<AuditLog> GenerateAuditLogsForDelete(BaseAuditLog baseAuditLog, object deletedEntity)
    {
        var result = new List<AuditLog>();
        
        var entityType = deletedEntity.GetType();
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(deletedEntity);
            
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
                    UserId = baseAuditLog.UserId,
                    ActionType = baseAuditLog.ActionType,
                    ProcessName = baseAuditLog.ProcessName,
                    EntityId = baseAuditLog.EntityId,
                    EntityType = baseAuditLog.EntityType,
                    
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
                        UserId = baseAuditLog.UserId,
                        ActionType = baseAuditLog.ActionType,
                        ProcessName = baseAuditLog.ProcessName,
                        EntityId = baseAuditLog.EntityId,
                        EntityType = baseAuditLog.EntityType,
                    
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