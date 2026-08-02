namespace XuanYu.Editor.Assets;

// D4-R1：托管事务状态机。
public enum SceneAssetHostingState
{
    Prepared,
    Activated,
    Completed,
    RolledBack,
    Failed
}
