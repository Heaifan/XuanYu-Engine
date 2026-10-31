using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

sealed partial class UiVmXycRuntime
{
    public XycDollyResult DollyCamera(double wheelDelta)
    {
        EnsureTerrainLoaded();
        return OnUi(() =>
        {
            var before = vm.ReadXycCamera();
            var handled = vm.DollyCamera(wheelDelta, out var constrained);
            var after = vm.ReadXycCamera();
            return new XycDollyResult(handled, before, after, constrained);
        });
    }
}
