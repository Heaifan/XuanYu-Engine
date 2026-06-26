using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.ShellV2.Composition;

namespace XuanYu.Engine.Editor.Windows.ShellV2;

/// <summary>EditorShellV2 最小骨架。旧 EditorShell 不受影响。第一阶段只接 Viewport。</summary>
public sealed partial class EditorShellV2 : UserControl
{
    EditorShellV2Context? _ctx;

    public EditorShellV2()
    {
        InitializeComponent();
        _ctx = EditorShellV2Composition.Build(this);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // V2 启动流程由 NativeHostInfoChanged 事件驱动（见 EditorShellV2Composition）
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _ctx?.Shutdown();
        base.OnDetachedFromVisualTree(e);
    }
}
