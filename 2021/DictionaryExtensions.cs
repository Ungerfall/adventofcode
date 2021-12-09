using System.Collections.Generic;

namespace _2021;

public static class DictionaryExtensions
{
    public static void AddOrIncrement<KType>(this Dictionary<KType, int> dic, KType key, int inc)
    {
        dic.TryGetValue(key, out int val);
        dic[key] = checked(val + inc);
    }

    public static void AddOrIncrement<KType>(this Dictionary<KType, long> dic, KType key, long inc)
    {
        dic.TryGetValue(key, out long val);
        dic[key] = checked(val + inc);
    }
}
