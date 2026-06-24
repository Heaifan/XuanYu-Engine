namespace XuanYu.Engine.Core.Results;

public readonly record struct EngineError
{
    private EngineError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public bool IsValid => !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Message);

    public static EngineError Create(string code, string message)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("错误代码不能为空。", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("错误信息不能为空。", nameof(message));
        }

        return new EngineError(code, message);
    }

    public override string ToString()
    {
        return $"{Code}：{Message}";
    }
}
