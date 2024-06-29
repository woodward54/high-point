using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Helper methods for the lists.
/// </summary>
public static class ListExtensions
{
    public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }

    public static IEnumerable<T> Mode<T>(this IEnumerable<T> input)
    {
        var dict = input.ToLookup(x => x);
        if (dict.Count == 0)
            return Enumerable.Empty<T>();
        var maxCount = dict.Max(x => x.Count());
        return dict.Where(x => x.Count() == maxCount).Select(x => x.Key);
    }

    public static void Shuffle<T>(this IList<T> list, Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}