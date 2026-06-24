namespace XuanYu.Engine.Editor.Input.Actions;

/// <summary>
/// 动作输入值的种类。
/// </summary>
public enum EditorInputValueKind
{
    /// <summary>单次触发（按键按下、按钮点击）。</summary>
    Trigger,

    /// <summary>二维连续位移（鼠标拖动）。</summary>
    PointerDelta,

    /// <summary>滚轮连续值。</summary>
    WheelDelta
}
