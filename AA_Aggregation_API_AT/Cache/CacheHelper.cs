using System.Reflection;

namespace AA_Aggregation_API_AT.Cache
{
    public static class CacheHelper
    {
        public static string ToCacheKey<T>(this T filter, params string[] include)
        {
            var dict = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Select(p => (p.Name, Value: p.GetValue(filter)))
                                .Where(x => x.Value != null && include.Contains(x.Name))
                                .OrderBy(x => x.Name)
                                .ToDictionary(x => x.Name, x => x.Value!.ToString()!);

            return string.Join("|", dict.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
