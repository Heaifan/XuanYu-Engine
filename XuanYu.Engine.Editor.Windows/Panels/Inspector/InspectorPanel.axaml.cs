using Avalonia.Controls;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Panels.Inspector;

public sealed partial class InspectorPanel : UserControl
{
    // ─── 控件引用 ──────────────────────────────────────────
    InspectorSelectionView _selView = null!;
    InspectorTransformView _xfrmView = null!;
    InspectorScrubInput _scrub = null!;
    readonly StackPanel? _xfrmSection;

    // ─── 事件 ──────────────────────────────────────────────
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubValueChanged;
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCompleted;
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCancelled;
    public string ScrubEntityId { get; set; } = "";
    public event Action<string, string, string>? TransformDraftChanged;
    public event Action<string, string, string>? TransformApplyRequested;
    public event Action? TransformResetRequested;
    public event Action? GroundPlacementRequested;
    public bool IsUpdatingTransformTexts => _xfrmView.IsUpdating;

    public InspectorPanel()
    {
        InitializeComponent();
        var e = this.FindControl<TextBlock>("EmptySelectionText");
        var d = this.FindControl<StackPanel>("SelectionDetails");
        var k = this.FindControl<TextBlock>("SelectionKindText");
        var n = this.FindControl<TextBlock>("SelectionNameText");
        var id = this.FindControl<TextBlock>("SelectionEntityIdText");
        var s = this.FindControl<TextBlock>("SelectionSourceText");
        var ts = this.FindControl<StackPanel>("TransformSection"); _xfrmSection = ts;
        var tx = this.FindControl<TextBox>("TransformXText");
        var ty = this.FindControl<TextBox>("TransformYText");
        var tz = this.FindControl<TextBox>("TransformZText");
        var err = this.FindControl<TextBlock>("TransformErrorText");
        var apply = this.FindControl<Button>("ApplyButton");
        var reset = this.FindControl<Button>("ResetButton");
        var gp = this.FindControl<Button>("GroundPlaceButton");
        var lx = this.FindControl<TextBlock>("ScrubLabelX");
        var ly = this.FindControl<TextBlock>("ScrubLabelY");
        var lz = this.FindControl<TextBlock>("ScrubLabelZ");

        _selView = new(e, d, k, n, id, s);
        _xfrmView = new(tx, ty, tz, err, apply, reset, gp);

        var getEid = () => ScrubEntityId;
        var getTxt = (TextBlock? lbl) => lbl == lx ? tx?.Text : lbl == ly ? ty?.Text : tz?.Text;
        Func<(string, string, string)> getTexts = () => _xfrmView.GetTexts();

        _scrub = new(lx, ly, lz, getEid, getTxt,
            (eid, ax, v) => ScrubValueChanged?.Invoke(eid, ax, v),
            (eid, ax, v) => ScrubCompleted?.Invoke(eid, ax, v),
            (eid, ax, v) => ScrubCancelled?.Invoke(eid, ax, v));
        _scrub.Attach();

        var binder = new InspectorTransformBinder(tx, ty, tz, apply, reset, gp, getTexts,
            (x, y, z) => TransformApplyRequested?.Invoke(x, y, z),
            () => TransformResetRequested?.Invoke(),
            () => GroundPlacementRequested?.Invoke());
        binder.Attach();

        foreach (var tb in new[] { tx, ty, tz })
            if (tb is not null) tb.TextChanged += (_, _) => { if (!_xfrmView.IsUpdating) { _xfrmView.ClearError(); _xfrmView.SetTransformDraftState(false, false, null); var (x, y, z) = _xfrmView.GetTexts(); TransformDraftChanged?.Invoke(x, y, z); } };
    }

    // ─── 公共方法 ──────────────────────────────────────────
    public void ShowSelection(EditorSelection sel) => _selView.ShowProjectFile(sel);
    public void ShowNoSelection() => _selView.ShowEmpty();
    public void ShowProjectFileSelection(EditorSelection sel) { _selView.ShowProjectFile(sel); if (_xfrmSection is not null) _xfrmSection.IsVisible = false; }
    public void ShowWorldEntitySelection(EditorSelection sel, string? entityId, Vector3d? position, string? sourcePath)
    { _selView.ShowWorldEntity(sel, entityId, position, sourcePath); if (_xfrmSection is not null) _xfrmSection.IsVisible = true; if (position is not null) _xfrmView.SetTexts(position.Value.X.ToString("F3", C), position.Value.Y.ToString("F3", C), position.Value.Z.ToString("F3", C)); else _xfrmView.SetTexts("", "", ""); }
    static readonly System.Globalization.CultureInfo C = System.Globalization.CultureInfo.InvariantCulture;

    public void SetPlacementMode(bool v) => _xfrmView.SetPlacementMode(v);
    public void SetTransformTexts(string x, string y, string z) => _xfrmView.SetTexts(x, y, z);
    public (string X, string Y, string Z) GetTransformTexts() => _xfrmView.GetTexts();
    public void SetGroundPlaceEnabled(bool v) => _xfrmView.SetGroundPlaceEnabled(v);
    public void SetTransformDraftState(bool canApply, bool canReset, string? error) => _xfrmView.SetTransformDraftState(canApply, canReset, error);
    public void ShowTransformError(string msg) => _xfrmView.ShowError(msg);
}
