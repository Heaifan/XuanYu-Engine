using Avalonia.Controls;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;
using XuanYu.Engine.Editor.Windows.Shell;

namespace XuanYu.Engine.Editor.Windows.Panels.Inspector;

public sealed partial class InspectorPanel : UserControl
{
    readonly InspectorSelectionView _selView = null!;
    internal InspectorTransformView _xfrmView = null!;
    readonly StackPanel? _xfrmSection;
    TransformInspectorSnapshot? _lastSnapshot;

    // 旧 Position 编辑事件（供现有管线使用）
    public event Action<string, string, string>? TransformDraftChanged;
    public event Action<string, string, string>? TransformApplyRequested;
    public event Action? TransformResetRequested;
    public event Action? GroundPlacementRequested;
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubValueChanged;
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCompleted;
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCancelled;
    // 新全 Transform 编辑事件
    public event Action<TransformEditRequest>? TransformAllApplyRequested;
    public string ScrubEntityId { get; set; } = "";
    public bool IsUpdatingTransformTexts => _xfrmView.IsUpdating;

    public InspectorPanel()
    {
        InitializeComponent();
        var e = F<TextBlock>("EmptySelectionText"); var d = F<StackPanel>("SelectionDetails");
        var k = F<TextBlock>("SelectionKindText"); var n = F<TextBlock>("SelectionNameText");
        var id = F<TextBlock>("SelectionEntityIdText"); var s = F<TextBlock>("SelectionSourceText");
        _xfrmSection = F<StackPanel>("TransformSection");
        var px = F<TextBox>("TransformXText"); var py = F<TextBox>("TransformYText"); var pz = F<TextBox>("TransformZText");
        var rx = F<TextBox>("RotationXText"); var ry = F<TextBox>("RotationYText"); var rz = F<TextBox>("RotationZText");
        var sx = F<TextBox>("ScaleXText"); var sy = F<TextBox>("ScaleYText"); var sz = F<TextBox>("ScaleZText");
        var err = F<TextBlock>("TransformErrorText");
        var apply = F<Button>("ApplyButton"); var reset = F<Button>("ResetButton"); var gp = F<Button>("GroundPlaceButton");
        var lx = F<TextBlock>("ScrubLabelX"); var ly = F<TextBlock>("ScrubLabelY"); var lz = F<TextBlock>("ScrubLabelZ");

        _selView = new(e, d, k, n, id, s);
        _xfrmView = new(px, py, pz, rx, ry, rz, sx, sy, sz, err, apply, reset, gp);
        _xfrmView._isUpdating = false;

        // Position 变更 → 旧管道
        foreach (var tb in new[] { px, py, pz })
            if (tb is not null) tb.TextChanged += (_, _) => { OnAnyTextChanged(); if (!_xfrmView.IsUpdating) { var t = (px?.Text ?? "", py?.Text ?? "", pz?.Text ?? ""); TransformDraftChanged?.Invoke(t.Item1, t.Item2, t.Item3); } };
        // Rotation/Scale 变更 → 新管道（启用按钮）
        foreach (var tb in new[] { rx, ry, rz, sx, sy, sz })
            if (tb is not null) tb.TextChanged += (_, _) => OnAnyTextChanged();

        if (apply is not null) apply.Click += (_, _) => { var t = (px?.Text ?? "", py?.Text ?? "", pz?.Text ?? ""); TransformApplyRequested?.Invoke(t.Item1, t.Item2, t.Item3); RequestAllApply(); };
        if (reset is not null) reset.Click += (_, _) => { _xfrmView.SetSnapshot(_lastSnapshot); TransformResetRequested?.Invoke(); };
        if (gp is not null) gp.Click += (_, _) => GroundPlacementRequested?.Invoke();

        // Scrub（Position 轴）
        var getEid = () => ScrubEntityId;
        var getTxt = (TextBlock? lbl) => lbl == lx ? px?.Text : lbl == ly ? py?.Text : pz?.Text;
        new InspectorTransformBinder(px, py, pz, apply, reset, gp,
            () => (px?.Text ?? "", py?.Text ?? "", pz?.Text ?? ""),
            (x, y, z) => TransformApplyRequested?.Invoke(x, y, z),
            () => TransformResetRequested?.Invoke(),
            () => GroundPlacementRequested?.Invoke()).Attach();
        new InspectorScrubInput(lx, ly, lz, getEid, getTxt,
            (eid, ax, v) => ScrubValueChanged?.Invoke(eid, ax, v),
            (eid, ax, v) => ScrubCompleted?.Invoke(eid, ax, v),
            (eid, ax, v) => ScrubCancelled?.Invoke(eid, ax, v)).Attach();
    }

    T? F<T>(string name) where T : Control => this.FindControl<T>(name);
    void OnAnyTextChanged() { if (_xfrmView.IsUpdating) return; _xfrmView.ClearError(); _xfrmView.SetTransformDraftState(true, false, null); }

    void RequestAllApply()
    { var t = _xfrmView.GetAllTexts(); TransformAllApplyRequested?.Invoke(new(null, TryParse(t.Px, t.Py, t.Pz), TryParse(t.Rx, t.Ry, t.Rz), TryParse(t.Sx, t.Sy, t.Sz))); }

    public void ShowSelection(EditorSelection sel) => _selView.ShowProjectFile(sel);
    public void ShowNoSelection() => _selView.ShowEmpty();
    public void ShowProjectFileSelection(EditorSelection sel)
    { _selView.ShowProjectFile(sel); if (_xfrmSection is not null) _xfrmSection.IsVisible = false; }
    public void ShowWorldEntitySelection(EditorSelection sel, string? entityId, Vector3d? pos, string? sourcePath, TransformInspectorSnapshot? fullTransform = null)
    { _selView.ShowWorldEntity(sel, entityId, pos, sourcePath); if (_xfrmSection is not null) _xfrmSection.IsVisible = true; if (fullTransform is not null) { _lastSnapshot = fullTransform; _xfrmView.SetSnapshot(fullTransform); } }
    public void SetPlacementMode(bool v) => _xfrmView.SetPlacementMode(v);
    public void SetGroundPlaceEnabled(bool v) => _xfrmView.SetGroundPlaceEnabled(v);
    public void ShowTransformError(string msg) => _xfrmView.ShowError(msg);
    public void SetTransformDraftState(bool canApply, bool canReset, string? error) => _xfrmView.SetTransformDraftState(canApply, canReset, error);
    public void SetTransformTexts(string x, string y, string z) => _xfrmView.SetSnapshot(new(ScrubEntityId, TryParse(x, y, z), Vector3d.Zero, new(1, 1, 1)));
    public (string X, string Y, string Z) GetTransformTexts() => (F<TextBox>("TransformXText")?.Text ?? "", F<TextBox>("TransformYText")?.Text ?? "", F<TextBox>("TransformZText")?.Text ?? "");

    static Vector3d TryParse(string x, string y, string z)
    {
        double.TryParse(x, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var dx);
        double.TryParse(y, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var dy);
        double.TryParse(z, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var dz);
        return new(dx, dy, dz);
    }
}
