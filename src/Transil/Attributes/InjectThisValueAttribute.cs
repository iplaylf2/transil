using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Transil.Attributes;

public sealed class InjectThisValueAttribute : StackValueAttribute {
    public override IEnumerable<CodeInstruction> Generate(TypeInfo instance)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0);       
    }
 }