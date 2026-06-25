#Requires -Version 5.1
$ErrorActionPreference = "Stop"

Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetWindowTextLength(IntPtr hWnd);

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
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray)] INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int left, top, right, bottom; }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT { public int x, y; }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public const uint INPUT_MOUSE = 0;
    public const uint MOUSEEVENTF_MOVE = 0x0001;
    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    public const int SW_RESTORE = 9;
    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;
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

function Send-MouseAbs($x, $y, $flags) {
    $sw = [Win32]::GetSystemMetrics([Win32]::SM_CXSCREEN)
    $sh = [Win32]::GetSystemMetrics([Win32]::SM_CYSCREEN)
    $absX = [int]($x * 65535 / $sw)
    $absY = [int]($y * 65535 / $sh)
    $inp = New-Object Win32+INPUT
    $inp.type = [Win32]::INPUT_MOUSE
    $inp.mi.dx = $absX
    $inp.mi.dy = $absY
    $inp.mi.dwFlags = [Win32]::MOUSEEVENTF_ABSOLUTE -bor $flags
    [void][Win32]::SendInput(1, @($inp), [System.Runtime.InteropServices.Marshal]::SizeOf([type][Win32+INPUT]))
}

function Move-MouseAbs($x, $y) { Send-MouseAbs $x $y ([Win32]::MOUSEEVENTF_MOVE) }
function Left-Down() { Send-MouseAbs 0 0 ([Win32]::MOUSEEVENTF_LEFTDOWN) }
function Left-Up() { Send-MouseAbs 0 0 ([Win32]::MOUSEEVENTF_LEFTUP) }

function Client-ToScreen($hwnd, $cx, $cy) {
    $pt = New-Object Win32+POINT
    $pt.x = $cx
    $pt.y = $cy
    [void][Win32]::ClientToScreen($hwnd, [ref]$pt)
    return $pt.x, $pt.y
}

Write-Host "启动编辑器..." -ForegroundColor Cyan
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "dotnet"
$psi.Arguments = "run --project XuanYu.Engine.Editor.Windows --no-build"
$psi.WorkingDirectory = "E:\MyDoc\project-VSCode\XuanYuEngine"
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.StandardOutputEncoding = [System.Text.Encoding]::UTF8
$proc = [System.Diagnostics.Process]::Start($psi)

# 实时输出转发到文件与控制台
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
                Write-Host $line
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
# 强制获得焦点：发送 Alt 键可以绕过某些焦点限制
$null = [Win32]::SetForegroundWindow($hwnd)
Start-Sleep -Milliseconds 500
[void][Win32]::SetForegroundWindow($hwnd)
Start-Sleep -Seconds 2

$rect = New-Object Win32+RECT
[void][Win32]::GetWindowRect($hwnd, [ref]$rect)
Write-Host "窗口位置: ($($rect.left),$($rect.top)) $($rect.right-$rect.left)x$($rect.bottom-$rect.top)"

# 点击层级面板第一个实体
$hx, $hy = Client-ToScreen $hwnd 80 150
Write-Host "点击层级: ($hx, $hy)"
Move-MouseAbs $hx $hy
Start-Sleep -Milliseconds 500
Left-Down
Start-Sleep -Milliseconds 200
Left-Up
Start-Sleep -Seconds 2

# 视口中心（默认布局）
$viewportCx = 260 + 44 + [Math]::Floor((1280 - 260 - 44 - 300) / 2)
$viewportCy = 44 + [Math]::Floor((800 - 44 - 200 - 28) / 2)
Write-Host "视口客户区中心: ($viewportCx, $viewportCy)"

# 移到视口中心
$cx, $cy = Client-ToScreen $hwnd $viewportCx $viewportCy
Move-MouseAbs $cx $cy
Start-Sleep -Seconds 1

# 尝试沿 X 轴手柄拖动
$hx, $hy = Client-ToScreen $hwnd ($viewportCx + 60) $viewportCy
Write-Host "尝试 Gizmo X 轴拖动: ($hx, $hy) -> +100px"
Move-MouseAbs $hx $hy
Start-Sleep -Milliseconds 300
Left-Down
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 10; $i++) {
    $nx, $ny = Client-ToScreen $hwnd ($viewportCx + 60 + $i * 10) $viewportCy
    Move-MouseAbs $nx $ny
    Start-Sleep -Milliseconds 16
}
Left-Up
Start-Sleep -Milliseconds 500

# 第二次尝试
Write-Host "第二次尝试拖动..."
Move-MouseAbs $cx $cy
Start-Sleep -Milliseconds 200
Left-Down
Start-Sleep -Milliseconds 100
for ($i = 0; $i -lt 20; $i++) {
    $nx, $ny = Client-ToScreen $hwnd ($viewportCx + $i * 8) ($viewportCy - $i * 4)
    Move-MouseAbs $nx $ny
    Start-Sleep -Milliseconds 16
}
Left-Up
Start-Sleep -Seconds 1

Write-Host "审计拖动完成，关闭编辑器..." -ForegroundColor Cyan
$proc.Kill()
$proc.WaitForExit(5000)
Remove-Job $outJob -Force
