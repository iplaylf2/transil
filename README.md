# Transil

Annotation-driven IL generation for HarmonyX transpilers
Automate complex HarmonyX patches with declarative code injection

## Features
- 🎯 **Precise IL hijacking** via `[ILHijackHandler]` attributes
- ⚡ **Auto-generated IL** based on method signatures
- 🧩 **Seamless integration** with Harmony's `CodeMatcher`
- 🔓 **Member injection** for fields, properties and arguments

## Basic Usage
```csharp
// 1. Define handler with injection attributes
[ILHijackHandler(HijackStrategy.InsertAdditional)]
public static float ProcessValue(
    [ConsumeStackValue] float original,
    [InjectMemberValue(MemberInjectionType.Field, "_multiplier")] float factor
)
{
    return original * factor;
}

// 2. Apply in transpiler
public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
{
    var matcher = new CodeMatcher(instructions);
    // ... target specific instructions ...
    ILManipulator.ApplyTransformation(matcher, ProcessValue, typeof(TargetClass));
    return matcher.InstructionEnumeration();
}
```

## Installation from GitHub Packages

This package is hosted in **GitHub Packages**. To use it:

1. **Add NuGet source**
   Add this feed URL to your NuGet sources:
   `https://nuget.pkg.github.com/iplaylf2/index.json`

2. **Authenticate with GitHub**
   You'll need a [Personal Access Token](https://github.com/settings/tokens) with **`read:packages`** scope.
   Follow GitHub's official guide to configure authentication:
   [Working with the NuGet registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry)