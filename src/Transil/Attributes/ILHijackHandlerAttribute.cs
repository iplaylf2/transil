using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Transil.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ILHijackHandlerAttribute(HijackStrategy strategy) : Attribute
{
    internal static ILHijackHandlerAttribute? GetHandlerAttribute(MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttribute<ILHijackHandlerAttribute>();
    }

    public HijackStrategy Strategy { get; } = strategy;

    public void ApplyHijack(CodeMatcher matcher, MethodInfo handler)
    {
        if (!handler.IsStatic)
        {
            throw new InvalidOperationException($"Handler method {handler.Name} must be static");
        }

        if (Strategy is HijackStrategy.ReplaceOriginal)
        {
            matcher.RemoveInstruction();
        }

        matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Call, handler));
    }
}

public enum HijackStrategy
{
    ReplaceOriginal,
    InsertAdditional
}