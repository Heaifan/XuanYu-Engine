using System.Text.Json.Serialization;

namespace XuanYu.Engine.Project.World.Documents;

/// <summary>组件文档基类。所有组件文档从此派生。</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "componentType")]
[JsonDerivedType(typeof(TransformComponentDocument), typeDiscriminator: "Transform")]
public abstract record WorldComponentDocument();

/// <summary>Transform 组件文档。保存实体位置（当前阶段仅支持 Position）。</summary>
public sealed record TransformComponentDocument(
    WorldVector3Document Position) : WorldComponentDocument();
