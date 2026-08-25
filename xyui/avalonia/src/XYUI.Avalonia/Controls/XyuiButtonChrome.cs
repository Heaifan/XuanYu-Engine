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
            // Padding 必须落在 ContentPresenter 而非 root Border：绑在 Border 上会把内容区
            // 连同 Action Edge 一起水平内缩，Edge 退化成悬空短线而非 Chrome 的底边。
            var root = new Border { ClipToBounds = true };
            root[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty];
            root[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
            root[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty];
            root[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
            var presenter = new ContentPresenter
            {
                HorizontalAlignment = horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };
            presenter[!ContentPresenter.PaddingProperty] = control[!TemplatedControl.PaddingProperty];
            presenter[!ContentPresenter.ContentProperty] = control[!Button.ContentProperty];
            presenter[!TextElement.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
            var grid = new Grid();
            grid.Children.Add(presenter);
            grid.Children.Add(new XyuiActionEdge());
            root.Child = grid;
            return root;
        });
}
