using System.Collections;

namespace App.Common.Extensions;

public static class ObjectExtensions
{
    public static string TranslateObjectValueToString(this object? value)
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
}