using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

internal sealed class VulkanFrameState
{
    public RenderProjection Projection { get; private set; }
    public bool HasProjection { get; private set; }
    public bool FrameDirty { get; private set; }
    public Vector3d RenderOrigin => _viewProjection?.RenderOrigin ?? default;
    ViewProjectionState? _viewProjection;

    public void Apply(RenderProjection projection)
    {
        Projection = projection;
        HasProjection = true;
        FrameDirty = true;
        _viewProjection = null;
    }

    public ViewProjectionState GetViewProjection(Extent2D extent, uint generation)
    {
        if (!HasProjection) throw new InvalidOperationException("Frame 尚未应用 RenderProjection。");
        var viewport = new ViewportState(0, 0, extent.Width, extent.Height,
            (int)extent.Width, (int)extent.Height, 1, generation);
        return _viewProjection = Projection.Camera.ToViewProjection(viewport);
    }

    public void Clear()
    {
        HasProjection = false;
        FrameDirty = false;
        _viewProjection = null;
    }

    public void MarkRecorded() => FrameDirty = false;
}
