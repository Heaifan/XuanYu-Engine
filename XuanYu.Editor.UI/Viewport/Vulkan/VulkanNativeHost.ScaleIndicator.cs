using System.ComponentModel;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    nint _scaleHwnd;
    UiVm? _scaleIndicatorVm;

    void CreateNativeScaleIndicator()
    {
        _scaleHwnd = Win32ViewportHost.CreateScaleIndicator(_hwnd);
        HookScaleIndicator();
    }

    void DestroyNativeScaleIndicator()
    {
        Win32ViewportHost.DestroyScaleIndicator(_scaleHwnd);
        _scaleHwnd = 0;
    }

    void HookScaleIndicator()
    {
        if (_scaleIndicatorVm is not null)
            _scaleIndicatorVm.PropertyChanged -= OnScaleIndicatorPropertyChanged;
        _scaleIndicatorVm = DataContext as UiVm;
        if (_scaleIndicatorVm is not null)
            _scaleIndicatorVm.PropertyChanged += OnScaleIndicatorPropertyChanged;
        UpdateNativeScaleIndicator();
    }

    void UnhookScaleIndicator()
    {
        if (_scaleIndicatorVm is null) return;
        _scaleIndicatorVm.PropertyChanged -= OnScaleIndicatorPropertyChanged;
        _scaleIndicatorVm = null;
    }

    void OnScaleIndicatorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(UiVm.IsScaleIndicatorVisible)
            or nameof(UiVm.ScaleIndicatorText)
            or nameof(UiVm.ScaleIndicatorWidthDip))
            UpdateNativeScaleIndicator();
    }

    void UpdateNativeScaleIndicator()
    {
        if (_scaleHwnd == 0) return;
        var vm = DataContext as UiVm;
        var size = Win32ViewportHost.GetClientSize(_hwnd);
        var dpi = GetDpiScale();
        Win32ViewportHost.UpdateScaleIndicator(_scaleHwnd,
            vm?.IsScaleIndicatorVisible == true, vm?.ScaleIndicatorText ?? "",
            vm?.ScaleIndicatorWidthDip ?? 80.0, dpi, size.Width, size.Height);
    }
}
