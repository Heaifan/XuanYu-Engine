#Requires -Version 5.1
$ErrorActionPreference = "Stop"

Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;
public class Win32 {
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int left, top, right, bottom; }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int x, y; }

    public const int SW_RESTORE = 9;
    public const uint WM_MOUSEMOVE = 0x0200;
    public const uint WM_LBUTTONDOWN = 0x0201;
    public const uint WM_LBUTTONUP = 0x0202;
    public const uint WM_RBUTTONDOWN = 0x0204;
    public const uint WM_RBUTTONUP = 0x0205;
    public const uint WM_MBUTTONDOWN = 0x0207;
    public const uint WM_MBUTTONUP = 0x0208;
}
"@

function Find-EditorWindow {
    $found = $null
    $sb = New-Object System.Text.StringBuilder 256
    $proc = [Win32+EnumWindowsProc] {
        param($hwnd, $lParam)
        if (-not [Win32]::IsWindowVisible($hwnd)) { return $true }
        $len = [Win32]::GetWindowTextLength($hwnd)
        if ($len -eq 0) { return $true }
        [void][Win32]::GetWindowText($hwnd, $sb, $sb.Capacity)
        $title = $sb.ToString()
        if ($title -like "*XuanYu Engine Editor*") {
            $script:found = $hwnd
            return $false
        }
        return $true
    }
    [void][Win32]::EnumWindows($proc, [IntPtr]::Zero)
    return $script:found
}

function Find-ViewportChild($parent) {
    $found = [IntPtr]::Zero
    $sb = New-Object System.Text.StringBuilder 256
    $proc = [Win32+EnumWindowsProc] {
        param($hwnd, $lParam)
        [void][Win32]::GetClassName($hwnd, $sb, $sb.Capacity)
        $cls = $sb.ToString()
        if ($cls -eq "XuanYuEngineVulkanViewportHost") {
            $script:found = $hwnd
            return $false
        }
        # 递归子窗口
        [void][Win32]::EnumChildWindows($hwnd, $proc, [IntPtr]::Zero)
        return $true
    }
    [void][Win32]::EnumChildWindows($parent, $proc, [IntPtr]::Zero)
    return $script:found
}

function Make-LParam($x, $y) {
    return [IntPtr](([uint32]$y -band 0xFFFF) -shl 16 -bor ([uint32]$x -band 0xFFFF))
}

function Post-Mouse($hwnd, $msg, $x, $y) {
    $lp = Make-LParam $x $y
    [void][Win32]::PostMessage($hwnd, $msg, [IntPtr]::Zero, $lp)
}

Write-Host "启动编辑器..." -ForegroundColor Cyan
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Write-Host "仓库根目录: $repoRoot" -ForegroundColor Gray
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "dotnet"
$psi.Arguments = "run --project XuanYu.Engine.Editor.Windows --no-build"
$psi.WorkingDirectory = $repoRoot.Path
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.StandardOutputEncoding = [System.Text.Encoding]::UTF8
$proc = [System.Diagnostics.Process]::Start($psi)

# 实时输出转发
$editorLog = Join-Path $psi.WorkingDirectory "tools\gizmo_drag_editor.log"
$outJob = Start-Job -ScriptBlock {
    param($p, $logPath)
    $sw = [System.IO.StreamWriter]::new($logPath, $false, [System.Text.Encoding]::UTF8)
    try {
        while (-not $p.HasExited) {
            $line = $p.StandardOutput.ReadLine()
            if ($line -ne $null) {
                $sw.WriteLine($line)
                $sw.Flush()
            }
        }
    } finally { $sw.Dispose() }
} -ArgumentList $proc, $editorLog

# 等待窗口
$hwnd = $null
for ($i = 0; $i -lt 60; $i++) {
    $hwnd = Find-EditorWindow
    if ($hwnd -ne $null) { break }
    Start-Sleep -Milliseconds 500
}
if ($hwnd -eq $null) {
    Write-Host "未找到编辑器窗口" -ForegroundColor Red
    $proc.Kill()
    exit 1
}

Write-Host "找到编辑器窗口 $hwnd" -ForegroundColor Green
[void][Win32]::ShowWindow($hwnd, [Win32]::SW_RESTORE)
Start-Sleep -Milliseconds 500
[void][Win32]::SetForegroundWindow($hwnd)
Start-Sleep -Seconds 2

$viewportHwnd = Find-ViewportChild $hwnd
if ($viewportHwnd -eq [IntPtr]::Zero) {
    Write-Host "未找到 Vulkan 视口子窗口" -ForegroundColor Red
    $proc.Kill()
    exit 1
}
Write-Host "找到视口子窗口 $viewportHwnd" -ForegroundColor Green

# 获取视口客户区尺寸
$client = New-Object Win32+RECT
[void][Win32]::GetClientRect($viewportHwnd, [ref]$client)
$vw = $client.right - $client.left
$vh = $client.bottom - $client.top
Write-Host "视口客户区: ${vw}x${vh}"

$cx = [Math]::Floor($vw / 2)
$cy = [Math]::Floor($vh / 2)

# 1) 中键旋转：DOWN -> MOVE -> UP
Write-Host "测试：中键旋转 ($cx, $cy) -> +90,+60"
Post-Mouse $viewportHwnd ([Win32]::WM_MBUTTONDOWN) $cx $cy
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 15; $i++) {
    $nx = $cx + $i * 6
    $ny = $cy + $i * 4
    Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $nx $ny
    Start-Sleep -Milliseconds 16
}
Post-Mouse $viewportHwnd ([Win32]::WM_MBUTTONUP) ($cx + 90) ($cy + 60)
Start-Sleep -Seconds 1

# 2) 点击视口中心选择实体
Write-Host "点击视口中心 ($cx, $cy)"
Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $cx $cy
Start-Sleep -Milliseconds 100
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONDOWN) $cx $cy
Start-Sleep -Milliseconds 100
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONUP) $cx $cy
Start-Sleep -Seconds 2

# 3) 尝试拖动 Gizmo X 轴手柄（中心右侧约 80 像素）
$hx = $cx + 80
$hy = $cy
Write-Host "尝试拖动 Gizmo ($hx, $hy) -> +120px"
Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $hx $hy
Start-Sleep -Milliseconds 200
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONDOWN) $hx $hy
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 12; $i++) {
    $nx = $hx + $i * 10
    Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $nx $hy
    Start-Sleep -Milliseconds 16
}
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONUP) ($hx + 120) $hy
Start-Sleep -Seconds 1

# 3) 再试一次斜向拖动
Write-Host "第二次斜向拖动"
Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $cx $cy
Start-Sleep -Milliseconds 200
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONDOWN) $cx $cy
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 20; $i++) {
    $nx = $cx + $i * 8
    $ny = $cy - $i * 4
    Post-Mouse $viewportHwnd ([Win32]::WM_MOUSEMOVE) $nx $ny
    Start-Sleep -Milliseconds 16
}
Post-Mouse $viewportHwnd ([Win32]::WM_LBUTTONUP) ($cx + 160) ($cy - 80)
Start-Sleep -Seconds 2

Write-Host "审计拖动完成，关闭编辑器..." -ForegroundColor Cyan
$proc.Kill()
$proc.WaitForExit(5000)
Remove-Job $outJob -Force
