using Avalonia.Controls;
using Avalonia.Layout;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    async Task<bool> ConfirmUnsavedBeforeContinue(UiVm vm)
    {
        if (!vm.IsSceneDirty) return true;
        var choice = await ShowUnsavedDialog();
        if (choice == "cancel") return false;
        if (choice == "discard") return true;
        return await SaveExistingOrPick(vm);
    }

    Task<string?> ShowUnsavedDialog()
    {
        var dialog = new Window
        {
            Title = "未保存的场景",
            Width = 360,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var text = new TextBlock
        {
            Text = "当前场景有未保存修改。",
            Margin = new Avalonia.Thickness(16)
        };
        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Avalonia.Thickness(16, 0, 16, 16)
        };
        buttons.Children.Add(Button("保存", "save", dialog));
        buttons.Children.Add(Button("不保存", "discard", dialog));
        buttons.Children.Add(Button("取消", "cancel", dialog));
        dialog.Content = new DockPanel { Children = { text, buttons } };
        DockPanel.SetDock(buttons, Dock.Bottom);
        return dialog.ShowDialog<string?>(this);
    }

    static Button Button(string text, string result, Window owner)
    {
        var button = new Button { Content = text, MinWidth = 72 };
        button.Click += (_, _) => owner.Close(result);
        return button;
    }
}
