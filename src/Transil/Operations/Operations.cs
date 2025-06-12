using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Transil.Attributes;
using Transil.Extensions;

namespace Transil.Operations;

public static class ILManipulator
{
    public static void ApplyTransformation<T>(
        CodeMatcher matcher,
        T handler,
        TypeInfo? instanceType = null) where T : Delegate
    {
        var l2Cache = L1Cache.GetOrCreateValue(handler);

        {
            if (l2Cache.TryGetValue(instanceType ?? NullTypeInfo, out var applyAction))
            {
                applyAction(matcher);

                return;
            }
        }

        var methodInfo = handler.GetMethodInfo();

        if (ILHijackHandlerAttribute.GetHandlerAttribute(methodInfo) is not { } handlerAttribute)
        {
            var methodDesc = $"{methodInfo.DeclaringType?.FullName ?? "unknown"}.{methodInfo.Name}";

            throw new InvalidOperationException(
                $"Handler method '{methodDesc}' must be decorated with [{nameof(ILHijackHandlerAttribute)}] "
                + "to specify hijack behavior"
            );
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

        {
            void applyAction(CodeMatcher matcher)
            {
                matcher.InsertAndAdvance(loadInstructions);

                handlerAttribute.ApplyHijack(matcher, methodInfo);
            }

            l2Cache.Add(instanceType ?? NullTypeInfo, applyAction);

            applyAction(matcher);
        }
    }

    private static readonly ConditionalWeakTable<Delegate, ConditionalWeakTable<TypeInfo, Action<CodeMatcher>>> L1Cache =
#if NETSTANDARD2_0
        new();
#else
        [];
#endif

    private static readonly TypeInfo NullTypeInfo = typeof(AsNull).GetTypeInfo();

    private sealed class AsNull { }
}
