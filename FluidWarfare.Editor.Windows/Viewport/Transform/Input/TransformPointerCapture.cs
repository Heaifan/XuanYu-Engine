using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>管理指针捕获和 SceneTool 仲裁。</summary>
public sealed class TransformPointerCapture
{
    private readonly Func<ViewportSceneToolPressResult> _onPress;
    private readonly Action _onRelease;
    private bool _captured;

    public TransformPointerCapture(Func<ViewportSceneToolPressResult> onPress, Action onRelease)
    {
        _onPress = onPress;
        _onRelease = onRelease;
    }

    public bool TryCapture()
    {
        if (_captured) return true;
        var result = _onPress();
        _captured = result == ViewportSceneToolPressResult.BeginDrag;
        return _captured;
    }

    public void Release()
    {
        if (!_captured) return;
        _captured = false;
        _onRelease();
    }

    public bool IsCaptured => _captured;
}
