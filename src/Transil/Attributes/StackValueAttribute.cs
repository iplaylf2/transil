using System.Reflection;
using HarmonyLib;

namespace Transil.Attributes;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public abstract class StackValueAttribute : Attribute
{
    internal static StackValueAttribute? GetStackValueAttribute(ParameterInfo parameterInfo)
    {
        var attributes = parameterInfo.GetCustomAttributes<StackValueAttribute>().Take(2).ToArray();

        return attributes.Length switch
        {
            0 => null,
            1 => attributes[0],
            _ => throw new InvalidOperationException(
                $"Parameter '{parameterInfo.Name}' in method '{parameterInfo.Member.Name}' " +
                $"has multiple StackValueAttribute applied. Only one is allowed.")
        };
    }

    public virtual IEnumerable<CodeInstruction> GenerateInstructions(TypeInfo? instanceType)
    {
        yield break;
    }
}