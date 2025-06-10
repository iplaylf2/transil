namespace Transil.Attributes;

public sealed class InjectFieldValueAttribute(string fieldName) : StackValueAttribute
{
    public string FieldName { get; } = fieldName;
}