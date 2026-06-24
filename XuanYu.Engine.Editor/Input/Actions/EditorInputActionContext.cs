namespace XuanYu.Engine.Editor.Input.Actions;

/// <summary>
/// 输入上下文，决定动作的生效范围。
/// ContextChain 中的顺序决定优先级，而非枚举的数值。
/// </summary>
public enum EditorInputActionContext
{
    /// <summary>键位捕获中：所有按键被捕获监听器接管。</summary>
    BindingCapture,

    /// <summary>文本输入框有焦点。</summary>
    TextInput,

    /// <summary>检查器 Transform 编辑中。</summary>
    InspectorTransform,

    /// <summary>活动编辑器工具（如地面放置、移动工具）。</summary>
    ActiveEditorTool,

    /// <summary>3D 视口导航。</summary>
    Viewport3D,

    /// <summary>全局：任何上下文之外生效。</summary>
    Global
}

/// <summary>
/// 输入上下文的优先级和过滤由 ContextChain 的列表顺序决定，
/// 而非枚举的整数值。索引 0 = 最高优先级，索引越大优先级越低。
/// </summary>
public static class EditorInputContextChain
{
    /// <summary>优先级从高到低的上下文链。</summary>
    public static IReadOnlyList<EditorInputActionContext> ContextChain { get; } = new[]
    {
        EditorInputActionContext.BindingCapture,
        EditorInputActionContext.TextInput,
        EditorInputActionContext.InspectorTransform,
        EditorInputActionContext.ActiveEditorTool,
        EditorInputActionContext.Viewport3D,
        EditorInputActionContext.Global,
    };

    /// <summary>
    /// 判断动作上下文在当前活动上下文中是否允许执行。
    /// Global 始终允许；其他上下文按 ContextChain 中的位置比较。
    /// </summary>
    public static bool IsContextAllowed(
        EditorInputActionContext actionContext,
        EditorInputActionContext activeContext)
    {
        // Global 动作始终允许
        if (actionContext == EditorInputActionContext.Global)
            return true;

        // 活动上下文在链中的位置必须 >= 动作上下文的位置才能放行
        // 位置越大 = 优先级越低，主动上下文可以放行同优先级或更低优先级的动作
        return IndexOf(activeContext) >= IndexOf(actionContext);
    }

    /// <summary>
    /// 上下文在 ContextChain 中的索引（0 = 最高优先级）。
    /// </summary>
    public static int IndexOf(EditorInputActionContext ctx)
    {
        for (var i = 0; i < ContextChain.Count; i++)
            if (ContextChain[i] == ctx) return i;
        return ContextChain.Count - 1; // fallback to lowest priority
    }
}
