using XuanYu.Core.Identity;

namespace XuanYu.Editor.Assets;

// D3：场景内实体 → 托管资产的最小绑定记录。
// 注意：Editor 层不引用 Render.Abstractions（arch-a-guard 强制 Editor 只引用
// Core/World），因此 RenderKey 不在本层派生；UI 层按 AssetId 派生稳定 RenderKey。
public readonly record struct SceneStaticModelBinding(
    EntityId EntityId,
    AssetId AssetId,
    string SourcePath);
