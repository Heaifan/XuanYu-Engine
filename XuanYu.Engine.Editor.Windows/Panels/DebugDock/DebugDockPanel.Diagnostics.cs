namespace XuanYu.Engine.Editor.Windows.Panels.DebugDock;

/// <summary>Partial：渲染诊断文本更新。</summary>
sealed partial class DebugDockPanel
{
    public void SetDiagnostics(string loader, string instance, string device, string nativeHost,
        string surface, string swapchain, string clear, string marker, string validation)
    {
        if (_diagLoader is not null) _diagLoader.Text = $"Vulkan 后端：{loader}";
        if (_diagInstance is not null) _diagInstance.Text = $"Instance：{instance}";
        if (_diagDevice is not null) _diagDevice.Text = $"Device：{device}";
        if (_diagNativeHost is not null) _diagNativeHost.Text = $"Native Host：{nativeHost}";
        if (_diagSurface is not null) _diagSurface.Text = $"Surface：{surface}";
        if (_diagSwapchain is not null) _diagSwapchain.Text = $"Swapchain：{swapchain}";
        if (_diagClear is not null) _diagClear.Text = $"Clear：{clear}";
        if (_diagMarker is not null) _diagMarker.Text = $"Marker：{marker}";
        if (_diagValidation is not null) _diagValidation.Text = $"Validation：{validation}";
    }

    public void SetScene3d(string scene3d, string camera, string grid, string unit, string drawCall)
    {
        if (_diagScene3d is not null) _diagScene3d.Text = $"Scene3D：{scene3d}";
        if (_diagCamera is not null) _diagCamera.Text = $"Camera：{camera}";
        if (_diagGrid is not null) _diagGrid.Text = $"Grid：{grid}";
        if (_diagUnit is not null) _diagUnit.Text = $"Unit：{unit}";
        if (_diagDrawCall is not null) _diagDrawCall.Text = $"DrawCall：{drawCall}";
    }
}
