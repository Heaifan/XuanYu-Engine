using System.Windows.Input;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isTerrainContext;
    public bool IsTerrainContext => _isTerrainContext;
    public bool IsRegionContext => !_isTerrainContext;
    public string ContextButtonLabel => _isTerrainContext ? "地形" : "区域";
    public string ContextToolbarButtonLabel => _isTerrainContext
        ? "地形" : DrawButtonLabel == "绘制" ? "区域" : DrawButtonLabel;
    public TerrainSourceData? TerrainSource { get; private set; }
    public TerrainWorld? TerrainWorld { get; private set; }
    public string TerrainStatus { get; private set; } = "未加载地形源。";
    public ICommand EnterTerrainContextCommand => new RelayCommand(_ => EnterTerrainContext());
    public ICommand EnterRegionContextCommand => new RelayCommand(_ => EnterRegionContext());
    public ICommand ImportTerrainCommand => new RelayCommand(_ => RequestTerrainImport());

    public void EnterTerrainContext()
    {
        if (_isTerrainContext) return;
        CancelActiveInput("切换地形上下文");
        _isTerrainContext = true;
        SelectTool("选择", logTool: false);
        RaiseTerrainContextBindings();
    }

    public void EnterRegionContext()
    {
        if (!_isTerrainContext) return;
        CancelActiveInput("切换区域上下文");
        _isTerrainContext = false;
        SelectTool("选择", logTool: false);
        RaiseTerrainContextBindings();
    }

    public bool ImportTerrainSource(string path)
    {
        try
        {
            var source = EsriAsciiGridTerrainReader.Read(path);
            var world = TerrainWorldFactory.FromSource(source);
            TerrainSource = source; TerrainWorld = world; TerrainStatus = "地形源已加载。";
            FooterMessage = TerrainStatus; FooterState = "状态：就绪";
            OnPropertyChanged(nameof(TerrainSource)); OnPropertyChanged(nameof(TerrainWorld));
            OnPropertyChanged(nameof(TerrainStatus));
            PublishSceneRenderSnapshot();
            return true;
        }
        catch (TerrainSourceReadException error) { return FailTerrainImport(error.Message); }
        catch (ArgumentException error) { return FailTerrainImport(error.Message); }
    }

    void RequestTerrainImport() => FileCommandRequested?.Invoke("导入DEM");

    bool FailTerrainImport(string message)
    {
        TerrainStatus = message; FooterMessage = message; FooterState = "状态：加载失败";
        OnPropertyChanged(nameof(TerrainStatus)); return false;
    }

    void RaiseTerrainContextBindings()
    {
        OnPropertyChanged(nameof(IsTerrainContext)); OnPropertyChanged(nameof(IsRegionContext));
        OnPropertyChanged(nameof(ContextButtonLabel)); OnPropertyChanged(nameof(ContextToolbarButtonLabel));
    }
}
