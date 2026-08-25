using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

// Batch 01 共享 Chrome 模板：Border 背景/边框/圆角 + 居中内容 + 底部 Action Edge 覆盖层。
internal static class XyuiButtonChrome
{
    internal static FuncControlTemplate<T> Create<T>(HorizontalAlignment horizontal) where T : Button =>
        new((control, scope) =>
        {
            var root = new Border();
            root[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty];
            root[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
            root[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty];
            root[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
            root[!Border.PaddingProperty] = control[!TemplatedControl.PaddingProperty];
            var presenter = new ContentPresenter
            {
                HorizontalAlignment = horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };
            presenter[!ContentPresenter.ContentProperty] = control[!Button.ContentProperty];
            presenter[!TextElement.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
            var grid = new Grid();
            grid.Children.Add(presenter);
            grid.Children.Add(new XyuiActionEdge());
            root.Child = grid;
            return root;
        });
}
