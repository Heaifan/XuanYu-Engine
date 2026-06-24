using XuanYu.Engine.Editor.Input.Actions;

namespace XuanYu.Engine.Editor.Input.Runtime;

/// <summary>
/// 输入事件匹配到的动作结果。
/// </summary>
public sealed record EditorInputMatch
{
    /// <summary>匹配的动作 ID，未匹配时为 null。</summary>
    public string? ActionId { get; init; }

    /// <summary>动作定义。</summary>
    public EditorInputActionDefinition? Definition { get; init; }

    /// <summary>值种类。</summary>
    public EditorInputValueKind ValueKind { get; init; }

    /// <summary>像素增量 X（PointerDelta 时使用）。</summary>
    public int DeltaX { get; init; }

    /// <summary>像素增量 Y（PointerDelta 时使用）。</summary>
    public int DeltaY { get; init; }

    /// <summary>滚轮增量。</summary>
    public float WheelDelta { get; init; }

    /// <summary>快照修订号。</summary>
    public int BindingRevision { get; init; }

    /// <summary>是否匹配成功。</summary>
    public bool IsMatch => ActionId is not null;

    /// <summary>空匹配（未命中任何动作）。</summary>
    public static EditorInputMatch NoMatch => new();
}
