using System.Globalization;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Panels.Inspector;

using XuanYu.Engine.Editor.Windows.Panels.Inspector;
namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

/// <summary>Inspector Transform 数值显示能力。不暴露 Panel 其他 API。</summary>
public sealed class InspectorTransformDisplay
{
    readonly InspectorPanel? _panel;

    public InspectorTransformDisplay(InspectorPanel? panel) => _panel = panel;

    public void SetPosition(Vector3d position)
    {
        GizmoDragProbe.MarkInspectorRefreshed();
        GizmoDragProbe.MarkUiRefreshed();
        GizmoDragProbe.Log("Inspector 刷新");
        _panel?.SetTransformTexts(
            position.X.ToString("F3", CultureInfo.InvariantCulture),
            position.Y.ToString("F3", CultureInfo.InvariantCulture),
            position.Z.ToString("F3", CultureInfo.InvariantCulture));
    }
}
