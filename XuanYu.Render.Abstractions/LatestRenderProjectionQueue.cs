namespace XuanYu.Render.Abstractions;

public sealed class LatestRenderProjectionQueue
{
    readonly object _gate = new();
    RenderProjectionResult _pending;
    bool _hasPending;

    public void Publish(RenderProjectionResult projection)
    {
        lock (_gate) { _pending = projection; _hasPending = true; }
    }

    public bool TryConsume(out RenderProjectionResult projection)
    {
        lock (_gate)
        {
            if (!_hasPending) { projection = default; return false; }
            projection = _pending; _hasPending = false; return true;
        }
    }
}
