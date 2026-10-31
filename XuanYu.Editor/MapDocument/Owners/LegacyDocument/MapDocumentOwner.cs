namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：当前地图状态所有者（最小状态机）。
// 无地图 / 新建未保存 / 已加载 / 已修改；不负责文件 IO、UI 与渲染。
public sealed class MapDocumentOwner
{
    public MapDocument? CurrentMap { get; private set; }
    public string? CurrentPath { get; private set; }
    public bool IsDirty { get; private set; }
    public string LastError { get; private set; } = "";

    public void New(MapDocument document)
    {
        CurrentMap = document;
        CurrentPath = null;
        IsDirty = true;
        LastError = "";
    }

    public void Load(string path, MapDocument document)
    {
        CurrentMap = document;
        CurrentPath = path;
        IsDirty = false;
        LastError = "";
    }

    public bool Modify(MapDocument document)
    {
        if (CurrentMap is null) return false;
        CurrentMap = document;
        IsDirty = true;
        return true;
    }

    public void Save(string path)
    {
        CurrentPath = path;
        IsDirty = false;
        LastError = "";
    }

    public void Unload()
    {
        CurrentMap = null;
        CurrentPath = null;
        IsDirty = false;
        LastError = "";
    }

    public void MarkError(string message) => LastError = message;
}
