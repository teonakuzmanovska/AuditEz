using System.Reflection;

namespace App.Common.Extensions;

public static class PropertyExtensions
{
    public static bool IsPropertyList(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsGenericType && typeof(List<>).IsAssignableFrom(propertyInfo.PropertyType.GetGenericTypeDefinition());
    }
    
    public static bool IsPropertyPrimitiveType(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string);
    }
}