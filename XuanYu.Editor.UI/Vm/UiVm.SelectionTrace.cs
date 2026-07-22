namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    static void TraceSelection(string stage, int depth, string detail)
    {
        if (depth <= 3 || depth % 10 == 0)
        {
            Console.Error.WriteLine(
                $"[DIAG Selection] {stage}; Depth={depth}; ThreadId={Environment.CurrentManagedThreadId}; {detail}");
        }
    }
}
