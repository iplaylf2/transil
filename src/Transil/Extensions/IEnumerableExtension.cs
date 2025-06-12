namespace Transil.Extensions;

internal static class IEnumerableExtension
{
    internal static IEnumerable<TAccumulate> Scan<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> accumulator)
    {
        TAccumulate state = seed;
        yield return state;

        foreach (var item in source)
        {
            state = accumulator(state, item);
            yield return state;
        }
    }
}