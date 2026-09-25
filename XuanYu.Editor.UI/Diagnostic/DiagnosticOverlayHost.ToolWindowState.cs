using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    double CardLeft { get; set; } = 12;
    double CardTop { get; set; } = 12;
    DiagnosticCardPlacementMode _cardPlacementMode = DiagnosticCardPlacementMode.Auto;

    void ResetCardPlacement()
    {
        _cardPlacementMode = DiagnosticCardPlacementMode.Auto;
        CardLeft = 12; CardTop = 12;
    }
}
