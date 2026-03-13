namespace Toems_UI;

public static class Extensions
{
    public static T? IfNotNull<T>(this T? obj, Action<T> action) where T : class
    {
        if (obj != null) action(obj);
        return obj;
    }
    
    public static async Task<T?> IfNotNullAsync<T>(this T? obj, Action<T> action) where T : class
    {
        if (obj != null) action(obj);
        return obj;
    }
}