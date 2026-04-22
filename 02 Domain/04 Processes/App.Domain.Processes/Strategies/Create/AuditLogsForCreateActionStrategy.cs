using System.Collections;
using App.Common.Extensions;
using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;

namespace App.Domain.Processes.Strategies.Create;

public class AuditLogsForCreateActionStrategy : IAuditLogsStrategy
{
    public ActionType ActionType => ActionType.Create;

    public List<AuditLog> Generate<T>(BaseAuditLog baseAuditLog, AuditLogRequest<T> request)
        where T : class
    {
        ValidateRequest(request);
        
        return GenerateAuditLogsForCreate(baseAuditLog, request.NewEntity!);
    }

    private void ValidateRequest<T>(AuditLogRequest<T> request) where T : class
    {
        var isCreateRequestValid = request.Context.Action is ActionType.Create && request.NewEntity is not null;
        
        if (isCreateRequestValid)
        {
            throw new InvalidAuditRequestException("Invalid request for Create action.");
        }
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
                    UserId = baseAuditLog.UserId,
                    ActionType = baseAuditLog.ActionType,
                    ProcessName = baseAuditLog.ProcessName,
                    EntityId = baseAuditLog.EntityId,
                    EntityType = baseAuditLog.EntityType,
                    
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
                        EntityId = baseAuditLog.EntityId,
                        EntityType = baseAuditLog.EntityType,
                        PropertyName = propertyInfo.Name,
                        NewPropertyValue = currentValueToString,
                        ActionType = baseAuditLog.ActionType,
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