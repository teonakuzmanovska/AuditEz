using System.Collections;
using App.Common.Extensions;
using App.Domain.Entities.Audit.Input;
using App.Domain.Entities.Audit.Output;
using App.Domain.Entities.Enum;

namespace App.Domain.Processes.Strategies;

internal class AuditLogsForUpdateActionStrategy<T> : AuditLogsForActionStrategy<T,UpdateAuditLogRequest<T>> where T : class
{
    protected override BaseAuditLog BuildBaseAuditLog(UpdateAuditLogRequest<T> request)
    {
        return new BaseAuditLog()
        {
            UserId = request.UserId,
            ProcessName = request.ProcessName,
            OriginatingEntityType = typeof(T).Name,
            OriginatingEntityId = request.NewEntity.GetEntityIdentifier()
        };
    }

    internal override List<AuditLog> Generate(UpdateAuditLogRequest<T> request)
    {
        base.Validate(request);
        
        var baseAuditLog = BuildBaseAuditLog(request);
        
        return GenerateAuditLogsForUpdate(baseAuditLog, request.OldEntity, request.NewEntity);
    }

    private List<AuditLog> GenerateAuditLogsForUpdate(BaseAuditLog baseAuditLog, object? existingEntity, object? updatedEntity)
    {
        var result = new List<AuditLog>();
        
        var entityType = updatedEntity?.GetType() ?? existingEntity?.GetType();
        
        if(entityType is null) return result;
        
        foreach (var propertyInfo in entityType.GetProperties())
        {
            if (propertyInfo.ShouldIgnoreProperty())
            {
                continue;
            }
            
            var currentValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(updatedEntity);
            var originalValue = entityType.GetProperty(propertyInfo.Name)?.GetValue(existingEntity);

            if (propertyInfo.IsPropertyList())
            {
                var updatedList = (currentValue as IList)?.Cast<object>().ToList();
                var existingList = (originalValue as IList)?.Cast<object>().ToList();
                
                if (updatedList.IsNullOrHasZeroElements() && existingList.IsNullOrHasZeroElements()) continue;
                
                var listType = existingList?[0].GetType() ?? updatedList![0].GetType();

                if (listType.IsPrimitive)
                {
                    result.Add(new AuditLog
                    {
                        OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                        EntityId = updatedEntity?.GetEntityIdentifier() ?? existingEntity!.GetEntityIdentifier(),
                    
                        UserId = baseAuditLog.UserId,
                        ActionType = ActionType.Update,
                        ProcessName = baseAuditLog.ProcessName,
                    
                        OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                        EntityType = entityType.Name,
                    
                        PropertyName = propertyInfo.Name,
                        NewPropertyValue = updatedList.TranslateObjectValueToString(),
                        OldPropertyValue = existingList.TranslateObjectValueToString()
                    });
                }
                
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
                        OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                        EntityId = existingEntity?.GetEntityIdentifier() ?? updatedEntity!.GetEntityIdentifier(),
                        
                        UserId = baseAuditLog.UserId,
                        ActionType = ActionType.Update,
                        ProcessName = baseAuditLog.ProcessName,
                        
                        OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                        EntityType = entityType.Name,
                        
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
            return AuditLogsForDeleteActionStrategy<T>.GenerateAuditLogsForListDelete(baseAuditLog, existingListProperty);
        }

        var propertyListType = updatedListProperty?[0].GetType() ?? existingListProperty![0].GetType();
        
        var unLoggedItemsFromOriginalList = existingListProperty?.ToList() ?? new List<object>();

        foreach (var propertyListElement in updatedListProperty!)
        {
            var originalEntity = existingListProperty!.FindMatchingProperty(propertyListElement);

            foreach (var propertyInfo in propertyListType.GetProperties())
            {
                if (propertyInfo.ShouldIgnoreProperty())
                {
                    continue;
                }
                
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
                            OriginatingEntityId = baseAuditLog.OriginatingEntityId,
                            EntityId = updatedListProperty?.GetEntityIdentifier() ?? existingListProperty!.GetEntityIdentifier(),

                            UserId = baseAuditLog.UserId,
                            ActionType = ActionType.Update,
                            ProcessName = baseAuditLog.ProcessName,
                            
                            OriginatingEntityType = baseAuditLog.OriginatingEntityType,
                            EntityType = propertyListType.Name,
                    
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
            result.AddRange(AuditLogsForDeleteActionStrategy<T>.GenerateAuditLogsForListDelete(baseAuditLog, unLoggedItemsFromOriginalList));
        }

        return result;
    }
}