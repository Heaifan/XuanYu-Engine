namespace XuanYu.Engine.Editor.Input.Bindings;

/// <summary>
/// 一个动作的绑定声明：主手势和可选手势。
/// </summary>
public sealed record EditorInputBinding
{
    /// <summary>动作 ID。</summary>
    public string ActionId { get; init; } = string.Empty;

    /// <summary>主手势。</summary>
    public EditorInputGesture? PrimaryGesture { get; init; }

    /// <summary>备选手势。</summary>
    public EditorInputGesture? SecondaryGesture { get; init; }

    /// <summary>动作定义（运行时时设置，不序列化）。</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public Actions.EditorInputActionDefinition? Definition { get; set; }
}
