namespace Transil.Kit;

public static class ScopeExtensions
{
    public static R Let<T, R>(this T it, Func<T, R> func) => func(it);

    public static T Also<T>(this T it, Action<T> action)
    {
        action(it);

        return it;
    }
}