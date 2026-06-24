namespace FluidWarfare.Editor.Windows.Shell.Lifecycle;

/// <summary>AttachRoute → Shell 的结果。AttachDispatched 表示是否首次触发 Post 时序。</summary>
public sealed record EditorShellAttachResult(bool AttachDispatched);
