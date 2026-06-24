using System.Text.Json.Serialization;

namespace XuanYu.Engine.Editor.Input.Bindings;

/// <summary>
/// 平台无关输入手势。
/// 键盘按键、鼠标拖拽或滚轮，附带可选修饰键。
/// </summary>
public sealed record EditorInputGesture
{
    /// <summary>输入设备类型。</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EditorInputDevice Device { get; init; }

    /// <summary>按键或鼠标按钮代码（与具体平台无关的抽象码）。</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>修饰键位标志。</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EditorInputModifiers Modifiers { get; init; } = EditorInputModifiers.None;

    /// <summary>手势种类。</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EditorInputGestureKind Kind { get; init; } = EditorInputGestureKind.KeyPress;

    /// <summary>手势的唯一签名（用于 O(1) 查表）。</summary>
    [JsonIgnore]
    public string Signature => BuildSignature();

    public EditorInputGesture() { }

    public EditorInputGesture(EditorInputDevice device, string code,
        EditorInputModifiers modifiers = EditorInputModifiers.None,
        EditorInputGestureKind kind = EditorInputGestureKind.KeyPress)
    {
        Device = device;
        Code = code;
        Modifiers = modifiers;
        Kind = kind;
    }

    private string BuildSignature()
    {
        var mod = Modifiers == EditorInputModifiers.None ? "" : Modifiers.ToString();
        return $"{mod}|{Kind}|{Device}|{Code}";
    }

    public string ToDisplayString()
    {
        var modText = Modifiers switch
        {
            EditorInputModifiers.Shift => "Shift+",
            EditorInputModifiers.Control => "Ctrl+",
            EditorInputModifiers.Alt => "Alt+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control => "Ctrl+Shift+",
            EditorInputModifiers.Shift | EditorInputModifiers.Alt => "Alt+Shift+",
            EditorInputModifiers.Control | EditorInputModifiers.Alt => "Ctrl+Alt+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control | EditorInputModifiers.Alt => "Ctrl+Alt+Shift+",
            _ => ""
        };
        var suffix = Kind switch
        {
            EditorInputGestureKind.MouseDrag => "拖动",
            EditorInputGestureKind.MouseWheel => "滚轮",
            _ => ""
        };
        var codeText = Device == EditorInputDevice.Wheel ? "滚轮" : Code;
        return $"{modText}{codeText}{suffix}";
    }
}

/// <summary>输入设备。</summary>
public enum EditorInputDevice { Keyboard, Mouse, Wheel }

/// <summary>修饰键。</summary>
[Flags]
public enum EditorInputModifiers { None = 0, Shift = 1, Control = 2, Alt = 4 }

/// <summary>手势种类。</summary>
public enum EditorInputGestureKind { KeyPress, MouseDrag, MouseWheel }
