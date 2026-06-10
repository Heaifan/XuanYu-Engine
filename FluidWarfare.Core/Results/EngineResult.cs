namespace FluidWarfare.Core.Results;

public readonly record struct EngineResult
{
    private EngineResult(bool isSuccess, EngineError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public EngineError? Error { get; }

    public static EngineResult Success()
    {
        return new EngineResult(true, null);
    }

    public static EngineResult Fail(EngineError error)
    {
        if (!error.IsValid)
        {
            throw new ArgumentException("错误对象无效。", nameof(error));
        }

        return new EngineResult(false, error);
    }

    public override string ToString()
    {
        return IsSuccess
            ? "EngineResult(成功)"
            : $"EngineResult(失败：{Error})";
    }
}
