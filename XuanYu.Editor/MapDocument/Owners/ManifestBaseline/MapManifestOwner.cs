namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：Map Document 当前状态所有者；失败加载不替换当前 Manifest。
public sealed class MapManifestOwner
{
    public MapManifest? CurrentManifest { get; private set; }
    public string? CurrentPath { get; private set; }
    public bool IsDirty { get; private set; }
    public string LastError { get; private set; } = "";

    public void SetBaseline(MapManifest manifest)
    {
        CurrentManifest = manifest;
        CurrentPath = null;
        IsDirty = false;
        LastError = "";
    }

    public void New(MapManifest manifest)
    {
        CurrentManifest = manifest;
        CurrentPath = null;
        IsDirty = true;
        LastError = "";
    }

    public void Load(string path, MapManifest manifest)
    {
        CurrentManifest = manifest;
        CurrentPath = path;
        IsDirty = false;
        LastError = "";
    }

    public bool Modify(MapManifest manifest)
    {
        if (CurrentManifest is null) return false;
        CurrentManifest = manifest;
        IsDirty = true;
        return true;
    }

    public void Save(string path)
    {
        CurrentPath = path;
        IsDirty = false;
        LastError = "";
    }

    public void MarkError(string message) => LastError = message;
}
