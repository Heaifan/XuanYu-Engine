namespace FluidWarfare.Editor.Input.Actions;

/// <summary>
/// 编辑器动作声明。动作 ID 是稳定字符串标识符，不依赖语言或按键。
/// </summary>
public sealed record EditorInputActionDefinition(
    string Id,
    string DisplayName,
    string Category,
    EditorInputActionContext Context,
    EditorInputValueKind ValueKind,
    bool IsUserConfigurable = true);
