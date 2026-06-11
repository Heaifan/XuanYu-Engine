using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Project;

public sealed partial class ProjectPanel : UserControl
{
    private StackPanel? _projectItemList;
    private TextBlock? _projectNameText;

    public event EventHandler<ProjectContentFolderSelection>? ProjectItemSelected;

    public ProjectPanel()
    {
        InitializeComponent();
        _projectNameText = this.FindControl<TextBlock>("ProjectNameText");
        _projectItemList = this.FindControl<StackPanel>("ProjectItemList");
    }

    public void ShowProject(string displayName, IEnumerable<ProjectContentFolderSelection> contentFolders)
    {
        _projectNameText ??= this.FindControl<TextBlock>("ProjectNameText");
        _projectItemList ??= this.FindControl<StackPanel>("ProjectItemList");

        if (_projectNameText is not null)
        {
            _projectNameText.Text = $"示例项目：{displayName}";
        }

        if (_projectItemList is null)
        {
            return;
        }

        _projectItemList.Children.Clear();

        foreach (var contentFolder in contentFolders)
        {
            var button = new Button
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Content = contentFolder.DisplayName
            };

            button.Click += (_, _) => SelectProjectItem(contentFolder);
            _projectItemList.Children.Add(button);
        }
    }

    public void ShowNoProject()
    {
        _projectNameText ??= this.FindControl<TextBlock>("ProjectNameText");
        _projectItemList ??= this.FindControl<StackPanel>("ProjectItemList");

        if (_projectNameText is not null)
        {
            _projectNameText.Text = "当前未打开项目";
        }

        _projectItemList?.Children.Clear();
    }

    private void SelectProjectItem(ProjectContentFolderSelection selection)
    {
        ProjectItemSelected?.Invoke(this, selection);
    }
}
