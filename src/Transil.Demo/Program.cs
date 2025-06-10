
using Transil.Attributes;

namespace Transil.Demo;

class Program
{
    public void Foo(
       [ConsumeStackValue] int foo,
      [InjectThisValue] int bar
    )
    {

    }
}