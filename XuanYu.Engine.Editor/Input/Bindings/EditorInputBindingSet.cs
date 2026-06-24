namespace XuanYu.Engine.Editor.Input.Bindings;

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

    /// <summary>
    /// 与另一个 BindingSet 的有效绑定是否完全相同。
    /// 比较 Preset 和所有 Override 的签名，用于判断是否有未保存的改动。
    /// </summary>
    public bool HasSameEffectiveBindingsAs(EditorInputBindingSet other)
    {
        if (Preset != other.Preset) return false;
        if (Overrides.Count != other.Overrides.Count) return false;

        // 按 (ActionId, Slot) 分组后比较 Gesture 签名
        var mine = Overrides.ToDictionary(o => (o.ActionId, o.Slot), o => o.Gesture?.Signature);
        var theirs = other.Overrides.ToDictionary(o => (o.ActionId, o.Slot), o => o.Gesture?.Signature);

        foreach (var (key, mySig) in mine)
        {
            if (!theirs.TryGetValue(key, out var theirSig))
                return false;
            if (mySig != theirSig)
                return false;
        }

        return true;
    }
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
