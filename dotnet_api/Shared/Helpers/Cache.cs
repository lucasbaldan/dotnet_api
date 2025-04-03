using Microsoft.Extensions.Caching.Memory;

namespace dotnet_api.Shared.Helpers;

public static class CacheHelpper
{
    private static void SetCache<T>(IMemoryCache cache,  string key, T data)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(30),
            Priority = CacheItemPriority.High,
        };
        cache.Set(key, data, cacheOptions);
    }

    public static T? GetCache<T>(IMemoryCache cache, string key)
    {
        cache.TryGetValue(key, out T? retorno);
        return retorno;
    }

    public static IEnumerable<T>? GetCaches<T>(IMemoryCache cache, string key)
    {
        cache.TryGetValue(key, out IEnumerable<T>? retorno);
        return retorno;
    }

    public static void RemoveCache(IMemoryCache cache, string key)
    {
        cache.Remove(key);
    }
}
