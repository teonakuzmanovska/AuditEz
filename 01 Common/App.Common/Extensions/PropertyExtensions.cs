using System.Reflection;
using App.Domain.Entities.Attributes;

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
    
    public static bool ShouldIgnoreProperty(this PropertyInfo propertyInfo)
    {
        return propertyInfo.IsDefined(typeof(AuditIgnoreAttribute), inherit: true) ||
               propertyInfo.IsDefined(typeof(EntityIdentifierAttribute), inherit: true);
    }
}