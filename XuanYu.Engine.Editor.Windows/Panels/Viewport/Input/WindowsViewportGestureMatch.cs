using XuanYu.Engine.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.Input;

/// <summary>
/// 手势签名构建与按钮名称映射。
/// 纯静态工具，无状态，供 WindowsViewportRawInputTranslate 调用。
/// </summary>
public static class WindowsViewportGestureMatch
{
    /// <summary>
    /// 构建手势签名。
    /// 必须与 EditorInputGesture.BuildSignature() 完全一致：
    /// None → ""（空串），非 None → 枚举字符串（"Control", "Shift" 等）。
    /// </summary>
    public static string BuildSignature(EditorInputDevice device, string code,
        EditorInputGestureKind kind, EditorInputModifiers modifiers)
    {
        var mod = modifiers == EditorInputModifiers.None ? "" : modifiers.ToString();
        return $"{mod}|{kind}|{device}|{code}";
    }

    /// <summary>
    /// Win32 按钮码到抽象名称映射。
    /// </summary>
    public static string ButtonCodeToName(int code) => code switch
    {
        1 => "Left",
        2 => "Right",
        3 => "Middle",
        4 => "Middle",
        5 => "X1",
        6 => "X2",
        _ => $"Button{code}"
    };
}
