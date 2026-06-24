namespace FluidWarfare.Engine.World.EntityPosition;

/// <summary>
/// World Entity 位置写入结果。
/// </summary>
public sealed record WorldEntityPositionWriteResult(
    bool IsSuccess,
    bool IsChanged,
    string Message,
    WorldEntityPositionChange? Change);
