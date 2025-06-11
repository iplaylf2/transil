using System.Reflection;
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
}
