using XuanYu.Engine.Render.Camera.Navigation;

namespace XuanYu.Engine.Editor.Windows.Viewport.Camera;

/// <summary>
/// 相机命令的区分联合类型。每种具体命令是一个 sealed record 子类。
/// Shell 构造命令并传递给 ViewportCameraRoute.Apply()。
/// </summary>
public abstract record ViewportCameraCommand
{
    private ViewportCameraCommand() { }

    /// <summary>轨道旋转（中键拖动）。DeltaYaw/Yaw 绕 Z，DeltaPitch 俯仰。</summary>
    public sealed record Orbit(float DeltaYaw, float DeltaPitch) : ViewportCameraCommand;

    /// <summary>平移（Shift+中键拖动）。</summary>
    public sealed record Pan(int DeltaX, int DeltaY, int ViewportHeight) : ViewportCameraCommand;

    /// <summary>推拉（Ctrl+中键拖动）。</summary>
    public sealed record Dolly(float DeltaPixels) : ViewportCameraCommand;

    /// <summary>滚轮缩放。</summary>
    public sealed record Zoom(float WheelNotches) : ViewportCameraCommand;

    /// <summary>查看全部。</summary>
    public sealed record FrameAll : ViewportCameraCommand;

    /// <summary>聚焦到包围盒中心。</summary>
    public sealed record FrameSelected(float CenterX, float CenterY, float CenterZ, float Radius) : ViewportCameraCommand;

    /// <summary>切换透视/正交。</summary>
    public sealed record ToggleProjection : ViewportCameraCommand;

    /// <summary>对齐到标准视图方向（前/后/左/右/上/下）。</summary>
    public sealed record SnapToView(SceneNavigationView View) : ViewportCameraCommand;
}
