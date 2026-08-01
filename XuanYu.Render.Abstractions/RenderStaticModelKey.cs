namespace XuanYu.Render.Abstractions;

public readonly record struct RenderStaticModelKey(string Value)
{
    public bool IsValid => !string.IsNullOrWhiteSpace(Value);
    public override string ToString() => Value;
}
