namespace XuanYu.Render.Abstractions;

public readonly record struct RenderVectorOverlayKey(string Value)
{
    public bool IsValid => !string.IsNullOrWhiteSpace(Value);
}
