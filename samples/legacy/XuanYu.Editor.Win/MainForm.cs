using System.Text;
using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.Win;

internal sealed class MainForm : Form
{
    readonly TextBox _reportBox = new();
    readonly Label _statusLabel = new();

    public MainForm()
    {
        Text = "XuanYu Editor Skeleton";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 980;
        Height = 720;
        Font = new Font("Segoe UI", 10f);

        var title = new Label
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(16, 12, 16, 0),
            Text = "XuanYu Editor Skeleton",
            Font = new Font(Font.FontFamily, 16f, FontStyle.Bold)
        };

        _statusLabel.Dock = DockStyle.Top;
        _statusLabel.Height = 28;
        _statusLabel.Padding = new Padding(16, 0, 16, 0);

        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };

        _reportBox.Dock = DockStyle.Fill;
        _reportBox.Multiline = true;
        _reportBox.ReadOnly = true;
        _reportBox.ScrollBars = ScrollBars.Vertical;
        _reportBox.Font = new Font("Consolas", 10f);
        _reportBox.BackColor = Color.White;

        panel.Controls.Add(_reportBox);
        Controls.Add(panel);
        Controls.Add(_statusLabel);
        Controls.Add(title);

        Load += (_, _) => ShowCoreReport();
    }

    void ShowCoreReport()
    {
        var report = CoreSelfTest.Run();
        _statusLabel.Text = report.IsPassed
            ? "Core self-test passed"
            : "Core self-test failed";
        _statusLabel.ForeColor = report.IsPassed ? Color.DarkGreen : Color.DarkRed;
        _reportBox.Text = BuildReportText(report);
    }

    static string BuildReportText(CoreSelfTestReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Current skeleton capabilities:");
        sb.AppendLine("- EntityId");
        sb.AppendLine("- EngineError / EngineResult");
        sb.AppendLine("- Vector3d");
        sb.AppendLine("- YawRotation");
        sb.AppendLine("- SimulationTime / TimeStep");
        sb.AppendLine("- EngineLogEntry / EngineLogLevel");
        sb.AppendLine();
        sb.AppendLine("Self-test results:");

        foreach (var item in report.Items)
        {
            sb.Append(item.Passed ? "[PASS] " : "[FAIL] ");
            sb.Append(item.Name);
            if (!string.IsNullOrWhiteSpace(item.Detail))
            {
                sb.Append(" - ");
                sb.Append(item.Detail);
            }
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine(report.IsPassed ? "Overall: PASS" : "Overall: FAIL");
        return sb.ToString();
    }
}
