using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Transil.Attributes;

public sealed class InjectArgumentValueAttribute(int index) : StackValueAttribute
{
    public int Index { get; } = index switch
    {
        < 0
        => throw new ArgumentOutOfRangeException(nameof(index), "Argument index cannot be negative"),
        > short.MaxValue
        => throw new ArgumentOutOfRangeException(nameof(index), $"Argument index cannot exceed {short.Max}"),
        var x => x
    };

    public override IEnumerable<CodeInstruction> Generate(TypeInfo instance)
    {
        yield return Index switch
        {
            var x when x <= 3 => LdargLookup[x],
            var x when x <= byte.MaxValue => new CodeInstruction(OpCodes.Ldarg_S, (byte)x),
            var x => new CodeInstruction(OpCodes.Ldarg, (short)x)
        };
    }

    private static readonly CodeInstruction[] LdargLookup =
    [
        new CodeInstruction(OpCodes.Ldarg_0),
        new CodeInstruction(OpCodes.Ldarg_1),
        new CodeInstruction(OpCodes.Ldarg_2),
        new CodeInstruction(OpCodes.Ldarg_3)
    ];
}
