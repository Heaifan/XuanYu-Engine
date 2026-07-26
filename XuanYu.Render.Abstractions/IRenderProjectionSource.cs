namespace XuanYu.Render.Abstractions;

public interface IRenderProjectionSource
{
    RenderProjectionResult RenderProjection { get; }
    event Action<RenderProjectionResult>? RenderProjectionChanged;
}
