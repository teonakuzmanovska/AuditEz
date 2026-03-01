namespace App.Common.Extensions;

public static class ListExtensions
{
    public static bool IsNullOrHasZeroElements<T>(this List<T>? list)
    {
        return list is null || list.Count == 0;
    }
    
    public static object? FindMatchingProperty(this List<object> list, object entity)
    {
        var entityId = GetEntityId(entity);
        
        if (entityId is null)
        {
            return null;
        }

        return list.FirstOrDefault(x =>
        {
            var candidateId = GetEntityId(x);
            return candidateId != null && candidateId.Equals(entityId);
        });
    }

    private static object? GetEntityId(object entity)
    {
        return entity.GetType()
            .GetProperty("Id")
            ?.GetValue(entity);
    }
}