namespace XuanYu.Engine.Project.World.Documents;

/// <summary>三维坐标文档模型。用于 Position / Rotation / Scale 的持久化。</summary>
public sealed record WorldVector3Document(
    float X,
    float Y,
    float Z)
{
    public static readonly WorldVector3Document Zero = new(0f, 0f, 0f);
}
