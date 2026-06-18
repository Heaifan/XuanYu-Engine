namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

public enum VulkanScene3dFrameReason
{
    SessionStart,
    CameraPan,
    CameraZoom,
    CameraReset,
    Resize,
    SelectionChanged,
    GroundCursorChanged,
    OverlayNavigationChanged,
    EntityTransformChanged,
    TransformPreview,
}
