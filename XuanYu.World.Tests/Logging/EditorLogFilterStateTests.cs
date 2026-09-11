using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

public sealed class EditorLogFilterStateTests
{
    [Fact]
    public void All_starts_with_three_severities_enabled()
    {
        var state = EditorLogFilterState.CreateDefault();

        Assert.True(state.Allows(New(EditorLogLevel.Info)));
        Assert.True(state.Allows(New(EditorLogLevel.Warning)));
        Assert.True(state.Allows(New(EditorLogLevel.Error)));
        Assert.True(state.IsAllSelected);
    }

    [Fact]
    public void Turning_off_one_severity_turns_off_all_without_hiding_other_enabled_levels()
    {
        var state = EditorLogFilterState.CreateDefault().ToggleSeverity(EditorLogLevel.Warning);

        Assert.False(state.IsAllSelected);
        Assert.True(state.Allows(New(EditorLogLevel.Info)));
        Assert.False(state.Allows(New(EditorLogLevel.Warning)));
        Assert.True(state.Allows(New(EditorLogLevel.Error)));
    }

    [Fact]
    public void Re_enabling_the_last_severity_restores_all()
    {
        var state = EditorLogFilterState.CreateDefault().ToggleSeverity(EditorLogLevel.Warning)
            .ToggleSeverity(EditorLogLevel.Warning);

        Assert.True(state.IsAllSelected);
    }

    [Fact]
    public void Search_and_source_are_additional_conditions()
    {
        var state = EditorLogFilterState.CreateDefault() with
        {
            Source = EditorLogSource.Render,
            SearchText = "resize"
        };

        Assert.True(state.Allows(New(EditorLogLevel.Info, EditorLogSource.Render, "Resize merged")));
        Assert.False(state.Allows(New(EditorLogLevel.Info, EditorLogSource.Render, "Frame presented")));
        Assert.False(state.Allows(New(EditorLogLevel.Info, EditorLogSource.Editor, "Resize merged")));
    }

    static LogEntry New(EditorLogLevel level, EditorLogSource source = EditorLogSource.Editor,
        string message = "message") => new("12:00", level, source, EditorLogCategory.Command, message);
}
