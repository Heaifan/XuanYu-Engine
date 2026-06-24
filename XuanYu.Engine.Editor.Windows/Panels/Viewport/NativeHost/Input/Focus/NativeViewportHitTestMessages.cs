namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Input.Focus;

/// <summary>Win32 命中测试消息识别。</summary>
static class NativeViewportHitTestMessages
{
    public const uint WmNcHitTest = 0x0084;
    public static bool IsHitTest(uint msg) => msg == WmNcHitTest;
}
