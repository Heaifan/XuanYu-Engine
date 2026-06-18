namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>Transform 应用操作状态。</summary>
public enum TransformApplyStatus { Success, NoChange, Failure }

/// <summary>Transform 应用结果。不含文案，日志由调用层生成。</summary>
public readonly record struct TransformApplyResult(TransformApplyStatus Status, string? Message)
{
    public bool IsSuccess => Status == TransformApplyStatus.Success;
    public static readonly TransformApplyResult SuccessResult = new(TransformApplyStatus.Success, null);
    public static TransformApplyResult Failure(string msg) => new(TransformApplyStatus.Failure, msg);
}
