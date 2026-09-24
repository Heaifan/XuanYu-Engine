using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;
using XuanYu.Editor.Input.Map;

namespace XuanYu.Editor.UI;

sealed class UiVmMapBackend(UiVm vm, GestureOwner owner, Func<ViewportState> viewport) : IMapEditingInputBackend
{
    readonly UiVm _vm = vm;
    readonly GestureOwner _owner = owner;
    readonly Func<ViewportState> _viewport = viewport;
    public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => p.Kind == EditorPointerEventKind.Pressed && _owner switch
    {
        GestureOwner.MapEdit => _vm.IsRegionEditMode && _vm.IsSelectTool,
        GestureOwner.Region => _vm.IsRegionDrawingTool,
        GestureOwner.Road => _vm.IsRoadDrawingTool,
        GestureOwner.Marker => _vm.IsMarkerPlacementTool,
        _ => false,
    };
    public void Begin(ViewportGestureContext c)
    {
        var (x, y) = (c.Input.Position.X, c.Input.Position.Y);
        if (_owner == GestureOwner.MapEdit) _vm.TryBeginMapGeometryPointer(x, y, _viewport());
        if (_owner == GestureOwner.Region) _vm.RegionDrawingPointerPressed(x, y, _viewport());
        if (_owner == GestureOwner.Road) _vm.RoadDrawingPointerPressed(x, y, _viewport());
        if (_owner == GestureOwner.Marker) _vm.MarkerPlacementPointerPressed(x, y, _viewport());
    }
    public void Update(ViewportGestureContext c)
    {
        var (x, y) = (c.Input.Position.X, c.Input.Position.Y);
        if (_owner == GestureOwner.MapEdit) _vm.PreviewMapGeometryPointer(x, y, _viewport());
        if (_owner == GestureOwner.Region) _vm.RegionDrawingPointerMoved(x, y, _viewport());
        if (_owner == GestureOwner.Road) _vm.RoadDrawingPointerMoved(x, y, _viewport());
    }
    public void Commit(ViewportGestureContext c)
    {
        var (x, y) = (c.Input.Position.X, c.Input.Position.Y);
        if (_owner == GestureOwner.MapEdit) _vm.CommitMapGeometryPointer(x, y, _viewport());
    }
    public void Cancel(ViewportCancellationContext c)
    {
        if (_owner == GestureOwner.MapEdit) _vm.CancelMapGeometryPointer(c.Reason.ToString());
        if (_owner == GestureOwner.Region) _vm.CancelRegionDrawingFromEscape();
        if (_owner == GestureOwner.Road) _vm.CancelRoadDrawingFromEscape();
    }
}
