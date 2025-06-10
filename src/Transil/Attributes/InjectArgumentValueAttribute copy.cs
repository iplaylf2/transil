namespace Transil.Attributes;

public sealed class InjectArgumentValueAttribute(int index) : StackValueAttribute
{
    public int Index { get; } = index;
}