using System.Collections;
using App.Common.Extensions;
using App.Domain.Entities.Audit.Base;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;
using App.Domain.Processes.Exceptions;
using App.Domain.Processes.Strategies.Delete;

namespace App.Domain.Processes.Strategies.Update;

public class AuditLogsForUpdateActionStrategy : IAuditLogsStrategy
{
    public ActionType ActionType => ActionType.Update;
    
    public List<AuditLog> Generate<T>(BaseAuditLog baseAuditLog, AuditLogRequest<T> request) where T : class
    {
        ValidateRequest(request);
        
        return GenerateAuditLogsForUpdate(baseAuditLog, request.OldEntity, request.NewEntity);
    }
    
    private void ValidateRequest<T>(AuditLogRequest<T> request) where T : class
    {
        var atLeastOneEntityIsNotNull = request.NewEntity is not null || request.OldEntity is not null;
        var isUpdateRequestValid = request.Context.Action is ActionType.Update && atLeastOneEntityIsNotNull && (request.OldEntity?.Id is not null || request.NewEntity?.Id is not null);
        
        if (isUpdateRequestValid)
        {
            throw new InvalidAuditRequestException("Invalid request for Update action.");
        }
    }

    private List<AuditLog> GenerateAuditLogsForUpdate(BaseAuditLog baseAuditLog, object? existingEntity, object? updatedEntity)
    {
        var result = new List<AuditLog>();
        
        var entityType = updatedEntity?.GetType() ?? existingEntity?.GetType();
        
        if(entityType is null) return result;
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(updatedEntity);
            var originalValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(existingEntity);

            if (propertyInfo.IsPropertyList())
            {
                var updatedList = (currentValue as IList)?.Cast<object>().ToList();
                var existingList = (originalValue as IList)?.Cast<object>().ToList();
                
                if (updatedList.IsNullOrHasZeroElements() && existingList.IsNullOrHasZeroElements()) continue;
                
                var auditLogsForListProperty = GenerateAuditLogsForListUpdate(baseAuditLog, existingList, updatedList);
                
                result.AddRange(auditLogsForListProperty);
            }
            
            else if (propertyInfo.IsPropertyPrimitiveType())
            {
                var originalValueToString = originalValue.TranslateObjectValueToString();
                var currentValueToString = currentValue.TranslateObjectValueToString();
                
                if (!string.Equals(originalValueToString, currentValueToString, StringComparison.Ordinal))
                {
                    result.Add(new AuditLog()
                    {
                        UserId = baseAuditLog.UserId,
                        ActionType = baseAuditLog.ActionType,
                        ProcessName = baseAuditLog.ProcessName,
                        EntityId = baseAuditLog.EntityId,
                        EntityType = baseAuditLog.EntityType,
                    
                        PropertyName = propertyInfo.Name,
                        OldPropertyValue = originalValueToString,
                        NewPropertyValue = currentValueToString,
                    });
                }
            }

            else
            {
                var auditLogsForProperty = GenerateAuditLogsForUpdate(baseAuditLog, originalValue, currentValue);

                if (!auditLogsForProperty.Any()) continue;

                result.AddRange(auditLogsForProperty);
            }

        }
        
        return result;
    }

    private List<AuditLog> GenerateAuditLogsForListUpdate(BaseAuditLog baseAuditLog, List<object>? existingListProperty, List<object>? updatedListProperty)
    {
        var result = new List<AuditLog>();
        
        if (updatedListProperty.IsNullOrHasZeroElements() && existingListProperty.IsNullOrHasZeroElements()) return result;
        
        if (updatedListProperty.IsNullOrHasZeroElements() && existingListProperty!.Count > 0)
        {
            return AuditLogsForDeleteActionStrategy.GenerateAuditLogsForListDelete(baseAuditLog, existingListProperty);
        }

        var propertyListType = updatedListProperty?[0].GetType() ?? existingListProperty![0].GetType();
        
        var unLoggedItemsFromOriginalList = existingListProperty?.ToList() ?? new List<object>();

        foreach (var propertyListElement in updatedListProperty!)
        {
            var originalEntity = existingListProperty!.FindMatchingProperty(propertyListElement);

            foreach (var propertyInfo in propertyListType.GetProperties())
            {
                var currentValue = propertyListType.GetProperty(propertyInfo.Name)?.GetValue(propertyListElement);
                var originalValue = originalEntity is null ? null : propertyListType.GetProperty(propertyInfo.Name)?.GetValue(originalEntity);
                
                if (propertyInfo.IsPropertyList())
                {
                    var updatedList = (currentValue as IList)?.Cast<object>().ToList();
                    var existingList = (originalValue as IList)?.Cast<object>().ToList();
                
                    if (updatedList.IsNullOrHasZeroElements() && existingList.IsNullOrHasZeroElements()) continue;
                
                    var auditLogsForListProperty = GenerateAuditLogsForListUpdate(baseAuditLog, existingList, updatedList);
                
                    result.AddRange(auditLogsForListProperty);
                }

                else if (propertyInfo.IsPropertyPrimitiveType())
                {
                    var originalValueToString = originalValue.TranslateObjectValueToString();
                    var currentValueToString = currentValue.TranslateObjectValueToString();
                
                    if (!string.Equals(originalValueToString, currentValueToString, StringComparison.Ordinal))
                    {
                        result.Add(new AuditLog()
                        {
                            UserId = baseAuditLog.UserId,
                            ActionType = baseAuditLog.ActionType,
                            ProcessName = baseAuditLog.ProcessName,
                            EntityId = baseAuditLog.EntityId,
                            EntityType = baseAuditLog.EntityType,
                    
                            PropertyName = propertyInfo.Name,
                            OldPropertyValue = originalValueToString,
                            NewPropertyValue = currentValueToString,
                        });
                    }
                }

                else
                {
                    var auditLogs = GenerateAuditLogsForUpdate(baseAuditLog, originalValue, currentValue).ToList();
                    if (!auditLogs.IsNullOrHasZeroElements()) result.AddRange(auditLogs);
                }

            }
            
            if (originalEntity != null)
            {
                unLoggedItemsFromOriginalList.Remove(originalEntity);
            }
        }

        if (!unLoggedItemsFromOriginalList.IsNullOrHasZeroElements())
        {
            result.AddRange(AuditLogsForDeleteActionStrategy.GenerateAuditLogsForListDelete(baseAuditLog, unLoggedItemsFromOriginalList));
        }

        return result;
    }
}