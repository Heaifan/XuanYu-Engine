namespace FluidWarfare.Editor.Windows.Panels.DebugDock;

/// <summary>Partial：性能计时更新。</summary>
sealed partial class DebugDockPanel
{
    public void SetPerformance(string instanceMs, string deviceMs, string swapchainMs,
        string clearMs, string markerMs, string scene3dMs)
    {
        if (_perfInstance is not null) _perfInstance.Text = $"Instance：{instanceMs} ms";
        if (_perfDevice is not null) _perfDevice.Text = $"Device：{deviceMs} ms";
        if (_perfSwapchain is not null) _perfSwapchain.Text = $"Swapchain：{swapchainMs} ms";
        if (_perfClear is not null) _perfClear.Text = $"Clear：{clearMs} ms";
        if (_perfMarker is not null) _perfMarker.Text = $"MarkerDraw：{markerMs} ms";
        if (_perfScene3d is not null) _perfScene3d.Text = $"Scene3D：{scene3dMs} ms";
    }
}
