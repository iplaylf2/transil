using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Transil.Attributes;

public sealed class InjectMemberValueAttribute(
    MemberInjectionType memberType,
    string memberName) : StackValueAttribute
{
    public MemberInjectionType MemberType { get; } = memberType;
    public string MemberName { get; } = memberName;

    public override IEnumerable<CodeInstruction> GenerateInstructions(TypeInfo? instanceType)
    {
        if (instanceType is null)
        {
            throw new ArgumentNullException(
                nameof(instanceType),
                "Instance type cannot be null when injecting member values"
            );
        }

        return MemberType switch
        {
            MemberInjectionType.Field
                => GenerateFieldLoad(instanceType, MemberName),

            _ => throw new NotSupportedException(
                $"Unsupported member type: {MemberType}. " +
                $"Supported types: {nameof(MemberInjectionType.Field)}")
        };
    }

    private static IEnumerable<CodeInstruction> GenerateFieldLoad(
        TypeInfo instance,
        string fieldName)
    {
        var fieldInfo = AccessTools.Field(instance, fieldName)
            ?? throw new MissingFieldException($"Field {fieldName} not found in {instance.FullName}");

        return
        [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, fieldInfo)
        ];
    }
}

public enum MemberInjectionType
{
    Field,
    // Property
}