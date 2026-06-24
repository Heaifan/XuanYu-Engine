using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Settings;

/// <summary>
/// 编辑器设置文档结构。
/// </summary>
public sealed record EditorSettingsDocument
{
    /// <summary>架构版本。</summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>输入绑定配置。</summary>
    public EditorInputBindingSet Input { get; init; } = new();

    /// <summary>是否为默认空文档。</summary>
    public bool IsDefault => SchemaVersion == 1 && Input.Preset == "blender"
        && (Input.Overrides is null || Input.Overrides.Count == 0);
}
