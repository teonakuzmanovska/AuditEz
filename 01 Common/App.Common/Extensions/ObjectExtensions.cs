using System.Collections;
using App.Domain.Entities.Attributes;

namespace App.Common.Extensions;

public static class ObjectExtensions
{
    public static string TranslateObjectValueToString(this object? value)
    {
        var result = value switch
        {
            null => string.Empty,
            
            string str => str,
            
            _ when value.GetType().IsPrimitive ||
                   value.GetType().IsValueType =>
                value.ToString() ?? string.Empty,
            
            IList list when value.GetType().IsGenericType &&
                            value.GetType().GetGenericTypeDefinition() == typeof(List<>) =>
                list.Count == 0
                    ? string.Empty
                    : string.Join(",", list.Cast<object>().Select(x => x?.ToString() ?? string.Empty)),
            
            _ => string.Empty
        };

        return result;
    }
    
    public static string GetEntityIdentifier(this object entity)
    {
        var identifierProperty = entity.GetType()
            .GetProperties()
            .Single(p =>
                Attribute.IsDefined(
                    p,
                    typeof(EntityIdentifierAttribute)));

        return identifierProperty.GetValue(entity)!.ToString()!;
    }
}