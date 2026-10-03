namespace XuanYu.Editor;

public enum EditorContextId { Region, Terrain }

public sealed record EditorContextSnapshot(long Revision, EditorContextId Context)
{
    public static EditorContextSnapshot Initial { get; } = new(1, EditorContextId.Region);
    public string ContextText => Context == EditorContextId.Terrain ? "地形" : "区域";
}
