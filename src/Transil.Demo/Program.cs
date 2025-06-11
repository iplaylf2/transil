
using HarmonyLib;
using Transil.Attributes;

namespace Transil.Demo;

class Program
{
  [ILHijackHandler(HijackStrategy.InsertAdditional)]
  public static int Foo(
    [ConsumeStackValue] int foo,
    [InjectArgumentValue(0)] String instance
  )
  {
    return foo;
  }

  public static void Demo(CodeMatcher matcher)
  {
    Transil.ApplyHijack(matcher, Foo);
  }
}