using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    static readonly HashSet<string> SafeInvocations = ["XYE.TOP.VIEW.ALL"];

    IReadOnlyList<XycUiControlState> ListControlsCore()
    {
        var controls = Controls().ToArray();
        var identities = controls.Select(XycUiControlLookup.Identity).ToArray();
        var counts = identities.Where(identity => identity.Id is not null)
            .GroupBy(identity => identity.Id!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
        return controls.Select((control, i) => Snapshot(control, identities[i],
            identities[i].Id is { } id && counts[id] > 1))
            .ToArray();
    }

    XycUiControlState InspectControlCore(string id)
    {
        var control = XycUiControlLookup.ResolveUnique(Controls(), id);
        return Snapshot(control, XycUiControlLookup.Identity(control), false);
    }

    XycUiControlInvokeState InvokeControlCore(string id, string operation)
    {
        var control = ResolveInvokable(id, operation);
        var before = InspectStateCore();
        var peer = ControlAutomationPeer.CreatePeerForElement(control);
        (peer.GetProvider<IInvokeProvider>() ?? throw new XycUiCommandException("UNSUPPORTED_OPERATION"))
            .Invoke();
        return new(Snapshot(control, XycUiControlLookup.Identity(control), false), before,
            InspectStateCore(), true,
            null!, null!);
    }

    async Task<XycUiControlInvokeState> InvokeControlWithEvidenceAsync(string id,
        string operation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = OnUi(() => ResolveInvokable(id, operation));
        var operationId = Guid.NewGuid().ToString("N");
        var beforeCapture = CaptureTop($"P1A-{operationId}-before.png");
        var result = OnUi(() => InvokeControlCore(id, operation));
        await Dispatcher.UIThread.InvokeAsync(() => FindTop().UpdateLayout(),
            DispatcherPriority.Render, cancellationToken);
        var afterCapture = CaptureTop($"P1A-{operationId}-after.png");
        return result with { BeforeCapture = beforeCapture, AfterCapture = afterCapture };
    }

    Control ResolveInvokable(string id, string operation)
    {
        var state = InspectControlCore(id);
        if (!state.Enabled) throw new XycUiCommandException("CONTROL_DISABLED");
        if (!state.Visible) throw new XycUiCommandException("CONTROL_NOT_VISIBLE");
        if (operation != "invoke" || !SafeInvocations.Contains(id))
            throw new XycUiCommandException("OPERATION_NOT_ALLOWED");
        return XycUiControlLookup.ResolveUnique(Controls(), id);
    }

    XycUiState InspectStateCore()
    {
        var vm = FindTop().DataContext as UiVm ?? window.DataContext as UiVm
            ?? throw new InvalidOperationException("生产 UI 状态尚未就绪。");
        return new(vm.ActiveTool, vm.IsSelectTool, vm.IsEditMode, vm.IsRegionEditMode,
            vm.FooterMessage, vm.FooterState, vm.NavigationCamera.Revision,
            vm.NavigationCamera.Position.X, vm.NavigationCamera.Position.Y,
            vm.NavigationCamera.Position.Z);
    }

}
