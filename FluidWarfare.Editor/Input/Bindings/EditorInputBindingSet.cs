namespace FluidWarfare.Editor.Input.Bindings;

/// <summary>
/// 完整的绑定集合：包含预设名和覆盖项。
/// </summary>
public sealed record EditorInputBindingSet
{
    /// <summary>预设名称（当前仅 "blender"）。</summary>
    public string Preset { get; init; } = "blender";

    /// <summary>用户覆盖项列表。</summary>
    public IReadOnlyList<EditorInputBindingOverride> Overrides { get; init; }
        = Array.Empty<EditorInputBindingOverride>();

    /// <summary>架构版本。</summary>
    public int SchemaVersion { get; init; } = 1;
}

/// <summary>
/// 用户对单个动作绑定的覆盖。
/// </summary>
public sealed record EditorInputBindingOverride
{
    /// <summary>动作 ID。</summary>
    public string ActionId { get; init; } = string.Empty;

    /// <summary>绑定槽位（"primary" 或 "secondary"）。</summary>
    public string Slot { get; init; } = "primary";

    /// <summary>新手势。设为 null 表示清除该槽位。</summary>
    public EditorInputGesture? Gesture { get; init; }
}
