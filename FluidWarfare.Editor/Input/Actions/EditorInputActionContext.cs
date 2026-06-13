namespace FluidWarfare.Editor.Input.Actions;

/// <summary>
/// 输入上下文，决定动作的生效范围。
/// 优先级从高到低。
/// </summary>
public enum EditorInputActionContext
{
    /// <summary>键位捕获中：所有按键被捕获监听器接管。</summary>
    BindingCapture,

    /// <summary>文本输入框有焦点。</summary>
    TextInput,

    /// <summary>检查器 Transform 编辑中。</summary>
    InspectorTransform,

    /// <summary>活动编辑器工具（如地面放置）。</summary>
    ActiveEditorTool,

    /// <summary>3D 视口导航。</summary>
    Viewport3D,

    /// <summary>全局：任何上下文之外生效。</summary>
    Global
}
