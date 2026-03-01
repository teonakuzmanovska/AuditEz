using System.Collections;

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
}