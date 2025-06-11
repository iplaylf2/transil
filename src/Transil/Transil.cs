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
            .Select(StackValueAttribute.GetStackValueAttribute)
            .Where(x => x is not null)
            .SelectMany(x => x!.GenerateInstructions(instanceType))
            .ToArray();

        matcher.InsertAndAdvance(loadInstructions);

        handlerAttribute.ApplyHijack(matcher, methodInfo);
    }
}
