using Avalonia.Controls;

namespace FluidWarfare.Editor.Windows.Panels.Project;

public sealed partial class ProjectPanel : UserControl
{
    private StackPanel? _projectItemList;
    private TextBlock? _projectNameText;

    public event EventHandler<string>? ProjectItemSelected;

    public ProjectPanel()
    {
        InitializeComponent();
        _projectNameText = this.FindControl<TextBlock>("ProjectNameText");
        _projectItemList = this.FindControl<StackPanel>("ProjectItemList");
    }

    public void ShowProject(string displayName, IEnumerable<string> categoryNames)
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

        foreach (var categoryName in categoryNames)
        {
            var button = new Button
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Content = categoryName
            };

            button.Click += (_, _) => SelectProjectItem(categoryName);
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

    private void SelectProjectItem(string itemName)
    {
        ProjectItemSelected?.Invoke(this, itemName);
    }
}
