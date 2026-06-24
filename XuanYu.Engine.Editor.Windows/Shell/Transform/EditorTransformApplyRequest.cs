using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;

namespace XuanYu.Engine.Editor.Windows.Shell.Transform;

/// <summary>Transform 提交/Preview/Cancel 的运行时依赖。Shell 在 InitTransformApplication 后设置。</summary>
public sealed record EditorTransformApplyDeps(
    EntityTransformPreview? Preview,
    EntityTransformCancel? Cancel,
    EntityTransformCommit? Commit);
