namespace XuanYu.Render.Abstractions;

public readonly record struct RenderProjectionResult(
    bool Success,
    RenderProjection Projection,
    string? FailureReason)
{
    public static RenderProjectionResult Ok(RenderProjection projection) =>
        new(true, projection, null);

    public static RenderProjectionResult Fail(string reason) =>
        new(false, default, reason);
}
