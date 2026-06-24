using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Scene3D.GroundCursor;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

partial class VulkanScene3dSession
{
    // ─── 公共属性 ───────────────────────────────────────────────
    public VulkanScene3dSessionStatus Status => _status;
    public int FrameIndex => _frameIndex;
    public int InstanceCreateCount => _instanceCreateCount;
    public int DeviceCreateCount => _deviceCreateCount;
    public int PipelineCreateCount => _pipelineCreateCount;
    public int BufferCreateCount => _bufferCreateCount;
    public int SwapchainGeneration => _swapchainGeneration;
    public int GridVertexCount => _gridVertexCount;
    public int UnitVertexCount => _unitVertexCount;
    public string? SelectedEntityId => _selectedEntityId;
    public PresentedCameraSnapshot LastPresentedSnapshot => _lastPresentedSnapshot;
    public Overlay.PresentedNavigationOverlaySnapshot LastPresentedOverlaySnapshot => _lastPresentedOverlaySnapshot;
    public int TransformRevision => _transformRevision;
    public VulkanGroundCursorInfo GroundCursorInfo => _cursorState.IsVisible
        ? new VulkanGroundCursorInfo(true, _cursorState.WorldPosition, _cursorState.Revision,
            _cursorVertexCount, _cursorState.IsVisible ? 1 : 0)
        : VulkanGroundCursorInfo.Hidden;

    /// <summary>Overlay 诊断信息。</summary>
    public Overlay.VulkanNavigationOverlayInfo OverlayInfo
    {
        get
        {
            if (_overlayResources is null || !_overlayResources.IsValid)
                return new Overlay.VulkanNavigationOverlayInfo(false, 0, 0, 0,
                    ViewportNavigationElement.None, "Overlay 不可用");
            return new Overlay.VulkanNavigationOverlayInfo(true,
                _lastOverlayVertexCount, Overlay.VulkanNavigationOverlayGeometry.MaxVertexCapacity,
                1, _overlayHovered, "Overlay 运行中");
        }
    }

    /// <summary>设置当前选中实体。</summary>
    public bool SetSelectedEntity(string? entityId)
    {
        if (_selectedEntityId == entityId) return false;
        _selectedEntityId = entityId;
        return true;
    }

    /// <summary>设置 Overlay 导航状态。</summary>
    public bool SetNavigationOverlayState(ViewportNavigationElement hovered, ViewportNavigationElement active)
    {
        if (_overlayHovered == hovered && _overlayActive == active) return false;
        _overlayHovered = hovered; _overlayActive = active; _overlayRevision++;
        return true;
    }

    /// <summary>更新实体绘制位置。</summary>
    public bool UpdateEntityPosition(string entityId, float x, float y, float z)
    {
        for (var i = 0; i < _cachedUnitDraws.Length; i++)
        {
            var draw = _cachedUnitDraws[i];
            if (draw.EntityId == entityId)
            {
                if (Math.Abs(draw.X - x) < 1e-6f && Math.Abs(draw.Y - y) < 1e-6f && Math.Abs(draw.Z - z) < 1e-6f)
                    return false;
                _cachedUnitDraws[i] = new VulkanScene3dUnitDrawInfo(entityId, x, y, z, draw.Scale);
                _transformRevision++;
                return true;
            }
        }
        return false;
    }

    /// <summary>设置地面落点标记。</summary>
    public bool SetGroundCursor(Vector3d? worldPosition)
    {
        if (_status != VulkanScene3dSessionStatus.Active) return false;
        return _cursorState.Set(worldPosition);
    }

    /// <summary>设置 Move Gizmo 覆盖层顶点。</summary>
    public void SetMoveGizmoVertices(Overlay.VulkanOverlayVertex[]? vertices) => _pendingGizmoVerts = vertices;
}
