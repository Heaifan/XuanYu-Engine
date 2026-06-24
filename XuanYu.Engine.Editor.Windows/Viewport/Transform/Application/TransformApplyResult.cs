namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

/// <summary>Transform 应用操作状态。</summary>
public enum TransformApplyStatus { Success, NoChange, Failure }

/// <summary>Transform 应用失败原因。不含中文文案，日志由调用层生成。</summary>
public enum TransformFailureReason
{
    None,
    RenderSceneSyncFailed,
    EntityNotFound,
}

/// <summary>Transform 应用结果。不含字符串文案。</summary>
public readonly record struct TransformApplyResult(
    TransformApplyStatus Status,
    TransformFailureReason FailureReason)
{
    public bool IsSuccess => Status == TransformApplyStatus.Success;
    public static readonly TransformApplyResult SuccessResult =
        new(TransformApplyStatus.Success, TransformFailureReason.None);
    public static readonly TransformApplyResult NoChangeResult =
        new(TransformApplyStatus.NoChange, TransformFailureReason.None);
    public static TransformApplyResult Failure(TransformFailureReason reason) =>
        new(TransformApplyStatus.Failure, reason);
}
