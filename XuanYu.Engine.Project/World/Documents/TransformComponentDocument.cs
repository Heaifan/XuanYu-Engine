using System.Text.Json.Serialization;

namespace XuanYu.Engine.Project.World.Documents;

/// <summary>组件文档基类。所有组件文档从此派生。</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "componentType")]
[JsonDerivedType(typeof(TransformComponentDocument), typeDiscriminator: "Transform")]
public abstract record WorldComponentDocument();

/// <summary>
/// Transform 组件文档。保存位置、旋转（欧拉角·度）、缩放。
/// 属性为 null 表示 JSON 中缺失，由校验或转换补默认值。
/// 缺 RotationDegrees → 默认 (0,0,0)；缺 Scale → 默认 (1,1,1)。兼容旧文件。
/// </summary>
public sealed record TransformComponentDocument : WorldComponentDocument
{
    public WorldVector3Document? Position { get; init; }
    public WorldVector3Document? RotationDegrees { get; init; }
    public WorldVector3Document? Scale { get; init; }

    public WorldVector3Document GetPositionOrDefault() =>
        Position ?? WorldVector3Document.Zero;

    public WorldVector3Document GetRotationDegreesOrDefault() =>
        RotationDegrees ?? WorldVector3Document.Zero;

    public WorldVector3Document GetScaleOrDefault() =>
        Scale ?? new WorldVector3Document(1f, 1f, 1f);
}
