namespace XuanYu.Editor.Assets;

public readonly record struct StaticModelPrimitive(
    int FirstIndex,
    int IndexCount,
    int BaseVertex,
    StaticModelColor BaseColorFactor);
