using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Runtime;

/// <summary>
/// 标准化的输入事件，由平台适配层产生。
/// </summary>
public sealed record EditorInputEvent
{
    /// <summary>输入设备。</summary>
    public EditorInputDevice Device { get; init; }

    /// <summary>按键/按钮代码。</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>修饰键状态。</summary>
    public EditorInputModifiers Modifiers { get; init; } = EditorInputModifiers.None;

    /// <summary>事件种类。</summary>
    public EditorInputEventKind EventKind { get; init; } = EditorInputEventKind.Press;

    /// <summary>鼠标像素 X。</summary>
    public int PixelX { get; init; }

    /// <summary>鼠标像素 Y。</summary>
    public int PixelY { get; init; }

    /// <summary>滚轮增量。</summary>
    public float WheelDelta { get; init; }

    /// <summary>手势签名（用于查表）。</summary>
    public string GestureSignature
    {
        get
        {
            var mod = Modifiers == EditorInputModifiers.None ? "" : Modifiers.ToString();
            var kind = EventKind == EditorInputEventKind.DragStart
                ? EditorInputGestureKind.MouseDrag.ToString()
                : EventKind == EditorInputEventKind.Wheel
                    ? EditorInputGestureKind.MouseWheel.ToString()
                    : EditorInputGestureKind.KeyPress.ToString();
            return $"{mod}|{kind}|{Device}|{Code}";
        }
    }
}

public enum EditorInputEventKind { Press, Release, DragStart, DragMove, DragEnd, Wheel }
