using System.Reflection;
using HarmonyLib;
using Transil.Attributes;
using Transil.Operations;

namespace Transil.Demo;

class Program
{
  [ILHijackHandler(HijackStrategy.InsertAdditional)]
  public static int Foo(
    [ConsumeStackValue] int foo,
    [InjectArgumentValue(0)] Bar instance,
    [InjectMemberValue(MemberInjectionType.Field, "xxx")] bool xxx
  )
  {
    return foo;
  }

  public static void Demo(CodeMatcher matcher)
  {
    ILManipulator.ApplyTransformation(matcher, Foo, typeof(Bar).GetTypeInfo());
  }
}

class Bar
{
  private readonly bool xxx;
}