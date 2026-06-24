using System.Globalization;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Windows.Panels.Inspector;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>Inspector Transform 数值显示能力。不暴露 Panel 其他 API。</summary>
public sealed class InspectorTransformDisplay
{
    readonly InspectorPanel? _panel;

    public InspectorTransformDisplay(InspectorPanel? panel) => _panel = panel;

    public void SetPosition(Vector3d position)
    {
        _panel?.SetTransformTexts(
            position.X.ToString("F3", CultureInfo.InvariantCulture),
            position.Y.ToString("F3", CultureInfo.InvariantCulture),
            position.Z.ToString("F3", CultureInfo.InvariantCulture));
    }
}
