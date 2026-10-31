using System.Windows.Input;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool IsTerrainContext => _contextState.Snapshot.Context == EditorContextId.Terrain;
    public bool IsRegionContext => !IsTerrainContext;
    public string ContextButtonLabel => ApplicationState.ContextLabel;
    public string ContextToolbarButtonLabel => ApplicationState.ToolbarLabel;
    public TerrainSourceData? TerrainSource { get; private set; }
    public TerrainWorld? TerrainWorld { get; private set; }
    public string TerrainStatus { get; private set; } = "未加载地形源。";
    public ICommand EnterTerrainContextCommand => new RelayCommand(_ => EnterTerrainContext());
    public ICommand EnterRegionContextCommand => new RelayCommand(_ => EnterRegionContext());
    public ICommand ImportTerrainCommand => new RelayCommand(_ => RequestTerrainImport());

    public void EnterTerrainContext()
    {
        if (IsDrawingTransactionActive) return;
        if (IsTerrainContext)
        {
            RaiseInspectorSelectionBindings();
            return;
        }
        CancelActiveInput("切换地形上下文");
        _contextState.Change(new ChangeEditorContextCommand(EditorContextId.Terrain));
        SelectTool("选择", logTool: false);
        RaiseTerrainContextBindings();
        RaiseInspectorSelectionBindings();
    }

    public void EnterRegionContext()
    {
        if (IsDrawingTransactionActive) return;
        if (!IsTerrainContext) return;
        CancelActiveInput("切换区域上下文");
        _contextState.Change(new ChangeEditorContextCommand(EditorContextId.Region));
        SelectTool("选择", logTool: false);
        RaiseTerrainContextBindings();
    }

    public bool ImportTerrainSource(string path)
    {
        try
        {
            var source = EsriAsciiGridTerrainReader.Read(path);
            var bounds = new TerrainGeoBounds(source.GeographicBounds.South,
                source.GeographicBounds.West, source.GeographicBounds.East,
                source.GeographicBounds.North);
            var tile = new TerrainElevationTile("terrain", bounds, source.Raster,
                source.Resolution, TerrainElevationUnit.Meter,
                TerrainVerticalDatum.Egm96Geoid, TerrainSourceFormat.NasademHgt);
            ActivateTerrainTiles(TerrainTileSet.Create([tile]));
            TerrainStatus = "地形源已加载。"; FooterMessage = TerrainStatus;
            OnPropertyChanged(nameof(TerrainStatus));
            return true;
        }
        catch (TerrainSourceReadException error) { return FailTerrainImport(error.Message, UiNotificationLevel.Error); }
        catch (ArgumentException error) { return FailTerrainImport(error.Message, UiNotificationLevel.Error); }
    }

    void RequestTerrainImport()
    {
        if (IsTerrainImporting) return;
        FileCommandRequested?.Invoke("导入DEM");
    }

    bool FailTerrainImport(string message, UiNotificationLevel level = UiNotificationLevel.Error)
    {
        if (level == UiNotificationLevel.Warning) NotifyWarning(message); else NotifyError(message);
        TerrainStatus = message; FooterMessage = message; FooterState = "状态：加载失败";
        OnPropertyChanged(nameof(TerrainStatus)); return false;
    }

    void RaiseTerrainContextBindings()
    {
        OnPropertyChanged(nameof(IsTerrainContext)); OnPropertyChanged(nameof(IsRegionContext));
        OnPropertyChanged(nameof(IsRegionEditMode));
        OnPropertyChanged(nameof(ContextButtonLabel)); OnPropertyChanged(nameof(ContextToolbarButtonLabel));
    }
}
