namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    static void TraceSelection(string stage, int depth, string detail)
    {
        if (depth <= 3 || depth % 10 == 0)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            Console.Error.WriteLine(
                $"{time} 【调试】【选择系统】选择投影诊断；阶段={stage}；深度={depth}；线程编号={Environment.CurrentManagedThreadId}；{detail}");
        }
    }
}
