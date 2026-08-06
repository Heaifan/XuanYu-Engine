using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD6LogPerformanceTests
{
    [Fact]
    public void Log_buffer_keeps_a_bounded_tail_window()
    {
        var buffer = new EditorLogBuffer();
        for (var i = 0; i < EditorLogBuffer.MaxEntries + 20; i++)
        {
            buffer.Add(NewEntry(i));
        }

        Assert.Equal(EditorLogBuffer.MaxEntries, buffer.All.Count);
        Assert.Equal("20", buffer.All[0].ContextId);
        Assert.Equal("519", buffer.All[^1].ContextId);
    }

    [Fact]
    public void Repeated_adjacent_entries_compact_in_place()
    {
        var buffer = new EditorLogBuffer();
        buffer.Add(NewEntry(1) with { Message = "Same", ContextId = "same" });
        buffer.Add(NewEntry(2) with { Message = "Same", ContextId = "same" });

        Assert.Single(buffer.All);
        Assert.Equal(2, buffer.All[0].RepeatCount);
    }

    static LogEntry NewEntry(int i) => new(
        "12:00",
        EditorLogLevel.Info,
        EditorLogSource.Editor,
        EditorLogCategory.Layout,
        "Message",
        ContextId: i.ToString());
}
