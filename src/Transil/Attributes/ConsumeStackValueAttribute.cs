namespace Transil.Attributes;

public sealed class ConsumeStackValueAttribute : StackValueAttribute
{
    public override bool RequiresInjection { get; } = false;
}