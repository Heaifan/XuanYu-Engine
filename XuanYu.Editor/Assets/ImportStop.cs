namespace XuanYu.Editor.Assets;

sealed class ImportStop : Exception
{
    public ImportStop(StaticModelImportErrorCode code, string message) : base(message) =>
        Code = code;

    public StaticModelImportErrorCode Code { get; }
}
