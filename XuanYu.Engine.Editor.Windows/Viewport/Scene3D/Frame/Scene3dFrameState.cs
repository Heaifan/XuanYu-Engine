namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;

/// <summary>
/// Scene3D 帧路径的核心状态。
/// FramePending — 帧合并闸门
/// SessionGeneration — 已排队的旧帧不得操作新 Session
/// IsDisposed — 已释放的 Session 跳过回调
/// </summary>
public sealed class Scene3dFrameState
{
    public bool FramePending;
    public int SessionGeneration;
    public bool IsDisposed;
    internal int RenderSeq;

    public bool TryAcquire()
    {
        if (FramePending || IsDisposed) return false;
        FramePending = true;
        return true;
    }

    public void Release() => FramePending = false;

    public void OnFrameRendered() => RenderSeq++;

    public void Dispose()
    {
        IsDisposed = true;
        FramePending = false;
    }

    public int NextGeneration() => ++SessionGeneration;
}
