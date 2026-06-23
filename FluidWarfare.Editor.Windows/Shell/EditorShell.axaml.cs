namespace FluidWarfare.Editor.Windows.Shell;

public sealed partial class EditorShell : UserControl
{
    EditorShellContext? _ctx;
    EditorShellLifecycle? _lifecycle;

    public EditorShell()
    {
        InitializeComponent();
        _ctx = EditorShellComposition.Build(this);
        _lifecycle = new EditorShellLifecycle(_ctx);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var req = _lifecycle!.BuildAttachRequest();
        _ctx!.AttachRoute.Attach(req);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var req = _lifecycle!.BuildDetachRequest();
        _lifecycle.ApplyDetachResult(_ctx!.DetachRoute.Detach(req));
        base.OnDetachedFromVisualTree(e);
    }
}
