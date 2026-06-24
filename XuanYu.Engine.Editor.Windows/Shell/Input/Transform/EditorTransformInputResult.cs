namespace XuanYu.Engine.Editor.Windows.Shell.Input.Transform;

/// <summary>Transform 输入路由的结果。Handled=true 表示事件已被变换逻辑消费。</summary>
public sealed record EditorTransformInputResult(bool Handled);
