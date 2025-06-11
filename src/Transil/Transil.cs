using System.Reflection;
using HarmonyLib;
using Transil.Attributes;

namespace Transil;

public static class Transil
{
    public static void ApplyHijack<T>(
        CodeMatcher matcher,
        T handler,
        TypeInfo? instanceType = null) where T : Delegate
    {
        var methodInfo = handler.GetMethodInfo();

        if (ILHijackHandlerAttribute.GetHandlerAttribute(methodInfo) is not { } handlerAttribute)
        {
            matcher.InsertAndAdvance(Transpilers.EmitDelegate(handler));

            return;
        }

        var loadInstructions = methodInfo
            .GetParameters()
            .Scan(
                (requiresInjection: false, stackValueAttribute: (StackValueAttribute?)null),
                (r, x) =>
                    {
                        var stackValueAttribute = StackValueAttribute.GetStackValueAttribute(x);
                        var requiresInjection = stackValueAttribute?.RequiresInjection ?? false;

                        if (r.requiresInjection && !requiresInjection)
                        {
                            throw new InvalidOperationException(
                                $"Parameter '{x.Name}' breaks continuous injection rule. "
                                + $"All parameters after a parameter requiring injection must also require injection. "
                                + $"Problematic method: {methodInfo.DeclaringType?.FullName}.{methodInfo.Name}"
                            );
                        }

                        return (requiresInjection, stackValueAttribute);
                    }
            )
            .Select(hasChecked => hasChecked.stackValueAttribute)
            .Where(x => x is not null)
            .SelectMany(x => x!.GenerateInstructions(instanceType))
            .ToArray();

        matcher.InsertAndAdvance(loadInstructions);

        handlerAttribute.ApplyHijack(matcher, methodInfo);
    }

    public static IEnumerable<TAccumulate> Scan<TSource, TAccumulate>(
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
