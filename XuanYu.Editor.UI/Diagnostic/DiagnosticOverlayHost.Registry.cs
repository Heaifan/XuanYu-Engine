using Avalonia.Threading;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void OnDiagnosticRegistryChanged(object? sender, EventArgs e)
    {
        if (_loaded) Dispatcher.UIThread.Post(Reconcile, DispatcherPriority.Render);
    }
}
