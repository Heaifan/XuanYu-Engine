using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateStepsLiveExamples()
    {
        var node1 = new XYStepNode("项目创建", XYStepState.Completed);
        var node2 = new XYStepNode("地形导入", XYStepState.Completed);
        var node3 = new XYStepNode("环境光照", XYStepState.Current);
        var node4 = new XYStepNode("参数校验", XYStepState.Warning);
        var node5 = new XYStepNode("打包导出", XYStepState.Pending) { CanNavigate = false };

        var horizontalSteps = new XYSteps(node1, node2, node3, node4, node5) { Width = 720 };
        var statusText = new TextBlock { Text = "当前阶段: [环境光照] (Current) · 阶段 4 包含 Warning 警告 · 阶段 5 处于锁定态 (CanNavigate=False)", Classes = { "xyui-text-caption" } };

        void SetupClick(XYStepNode target, string name, XYStepState state)
        {
            target.NavigationRequested += (_, _) =>
            {
                statusText.Text = $"已切换至: [{name}] · 状态: {state}";
                foreach (var n in new[] { node1, node2, node3, node4 })
                {
                    if (ReferenceEquals(n, target)) n.State = XYStepState.Current;
                    else if (Array.IndexOf(new[] { node1, node2, node3, node4 }, n) < Array.IndexOf(new[] { node1, node2, node3, node4 }, target))
                        n.State = XYStepState.Completed;
                    else n.State = XYStepState.Pending;
                }
            };
        }
        SetupClick(node1, "项目创建", XYStepState.Completed);
        SetupClick(node2, "地形导入", XYStepState.Completed);
        SetupClick(node3, "环境光照", XYStepState.Current);
        SetupClick(node4, "参数校验", XYStepState.Warning);

        var v1 = new XYStepNode("1. 基础配置", XYStepState.Completed);
        var v2 = new XYStepNode("2. 资源映射", XYStepState.Current);
        var v3 = new XYStepNode("3. 冲突检查", XYStepState.Error);
        var v4 = new XYStepNode("4. 最终发布", XYStepState.Pending) { CanNavigate = false };
        var verticalSteps = new XYSteps(v1, v2, v3, v4) { Orientation = XYStepsOrientation.Vertical, Width = 260 };

        var root = new StackPanel
        {
            Spacing = 16,
            Children =
            {
                new TextBlock { Text = "横向向导流程 (支持点击节点实时推进/回退):", Classes = { "xyui-text-label" } },
                horizontalSteps,
                statusText,
                new TextBlock { Text = "纵向步骤排布 (含 Error 错误状态与锁定步骤):", Classes = { "xyui-text-label" } },
                verticalSteps
            }
        };
        return WrapCard(root, "步骤向导 · 五态表达、连通导线与横竖双向排布");
    }

    static Control CreateStepsComposition()
    {
        var step1 = new XYStepNode("拓扑配置", XYStepState.Completed);
        var step2 = new XYStepNode("标高平滑", XYStepState.Current);
        var step3 = new XYStepNode("材质烘焙", XYStepState.Pending);
        var steps = new XYSteps(step1, step2, step3) { Width = 520 };

        var body = new Border
        {
            Classes = { "xyui-surface-panel-alt" },
            Padding = new(16),
            MinHeight = 90,
            Child = new TextBlock { Text = "阶段 2：正在平滑选定区域标高曲线...\n算法收敛步长：0.025 DIP · 采样顶点：12,480", Classes = { "xyui-text-code" } }
        };

        var prevBtn = new XYButton { Content = "上一步", Variant = XyuiButtonVariant.Secondary, Width = 80 };
        var nextBtn = new XYButton { Content = "下一步", Variant = XyuiButtonVariant.Primary, Width = 80 };
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Right, Children = { prevBtn, nextBtn } };

        var wizard = new StackPanel { Spacing = 14, Width = 540, Children = { steps, body, actions } };
        return WrapCard(wizard, "向导式任务对话框 · 步骤导航与多阶段操作驱动");
    }
}
