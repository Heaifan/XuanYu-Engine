[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][int]$ProcessId,
    [int]$WheelCount = 40,
    [int]$OrbitSteps = 8,
    [int]$ReverseWheelCount = 3,
    [int]$TimeoutSeconds = 600
)

$ErrorActionPreference = 'Stop'
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class Wp2NativeInput {
  [StructLayout(LayoutKind.Sequential)] public struct MouseInput {
    public int dx, dy; public uint mouseData, flags, time; public UIntPtr extra;
  }
  [StructLayout(LayoutKind.Explicit)] public struct InputUnion {
    [FieldOffset(0)] public MouseInput mouse;
  }
  [StructLayout(LayoutKind.Sequential)] public struct Input {
    public uint type; public InputUnion value;
  }
  [StructLayout(LayoutKind.Sequential)] public struct Rect {
    public int left, top, right, bottom;
  }
  public delegate bool EnumWindow(IntPtr hwnd, IntPtr state);
  [DllImport("user32.dll", SetLastError=true)] static extern uint SendInput(uint count, Input[] inputs, int size);
  [DllImport("user32.dll", SetLastError=true)] static extern bool SetCursorPos(int x, int y);
  [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
  [DllImport("user32.dll")] static extern bool EnumChildWindows(IntPtr parent, EnumWindow callback, IntPtr state);
  [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hwnd);
  [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
  [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hwnd, int command);
  [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hwnd, IntPtr insertAfter,
    int x, int y, int width, int height, uint flags);
  [DllImport("user32.dll", SetLastError=true)] static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
  [DllImport("user32.dll")] static extern bool AttachThreadInput(uint attach, uint attachTo, bool value);
  [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
  public static int[] Bounds(IntPtr hwnd) {
    if (!GetWindowRect(hwnd, out var r)) throw new InvalidOperationException("GetWindowRect failed");
    return new[] { r.left, r.top, r.right, r.bottom };
  }
  public static IntPtr FindViewport(IntPtr parent, int width, int height, out int[] bounds) {
    IntPtr selected = IntPtr.Zero; var best = Int32.MaxValue; int[] selectedBounds = null;
    EnumChildWindows(parent, (child, _) => {
      if (!GetWindowRect(child, out var r)) return true;
      var w = r.right-r.left; var h = r.bottom-r.top;
      var score = Math.Abs(w-width)+Math.Abs(h-height);
      if (score < best) { best=score; selected=child; selectedBounds=new[] { r.left,r.top,r.right,r.bottom }; }
      return true;
    }, IntPtr.Zero);
    if (selected == IntPtr.Zero || best > 20)
      throw new InvalidOperationException("Viewport child HWND matching the physical viewport was not found.");
    bounds=selectedBounds; return selected;
  }
  public static void Activate(IntPtr hwnd) {
    ShowWindow(hwnd, 9);
    SetWindowPos(hwnd, new IntPtr(-1), 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0040);
    var foreground = GetForegroundWindow();
    var foregroundThread = GetWindowThreadProcessId(foreground, out _);
    var currentThread = GetCurrentThreadId();
    var attached = foregroundThread != 0 && foregroundThread != currentThread &&
      AttachThreadInput(currentThread, foregroundThread, true);
    SetForegroundWindow(hwnd);
    if (attached) AttachThreadInput(currentThread, foregroundThread, false);
    System.Threading.Thread.Sleep(300);
  }
  public static void MoveTo(int x, int y) {
    if (!SetCursorPos(x, y)) throw new InvalidOperationException("SetCursorPos failed");
  }
  public static void Mouse(uint flags, uint data) {
    var input = new Input { type = 0, value = new InputUnion { mouse = new MouseInput { mouseData=data, flags=flags } } };
    if (SendInput(1, new[] { input }, Marshal.SizeOf<Input>()) != 1)
      throw new InvalidOperationException("SendInput failed: " + Marshal.GetLastWin32Error());
  }
  public static void Wheel(int delta) => Mouse(0x0800, unchecked((uint)delta));
}
'@

$readyPath = Join-Path $OutputDirectory 'native-input.ready.json'
$timer = [Diagnostics.Stopwatch]::StartNew()
while (-not (Test-Path -LiteralPath $readyPath)) {
    if ($timer.Elapsed.TotalSeconds -gt $TimeoutSeconds) { throw 'Native replay did not become ready.' }
    Start-Sleep -Milliseconds 50
}
$ready = Get-Content -LiteralPath $readyPath -Raw | ConvertFrom-Json
if ($ready.ProcessId -ne $ProcessId) { throw 'Native replay process identity mismatch.' }
$process = Get-Process -Id $ProcessId
$window = [IntPtr]$process.MainWindowHandle
if ($window -eq [IntPtr]::Zero) { throw 'Native replay main HWND is unavailable.' }
[Wp2NativeInput]::Activate($window)
$bounds = $null
$viewportHwnd = [Wp2NativeInput]::FindViewport($window,
    [int]$ready.Viewport.PhysicalWidth, [int]$ready.Viewport.PhysicalHeight, [ref]$bounds)
$width = $bounds[2] - $bounds[0]; $height = $bounds[3] - $bounds[1]
$wheelX = $bounds[0] + [int]($width * 0.75)
$wheelY = $bounds[1] + [int]($height * 0.48)
$orbitX = $bounds[0] + [int]($width * 0.70)
$orbitY = $bounds[1] + [int]($height * 0.55)
[Wp2NativeInput]::MoveTo($wheelX, $wheelY)
Start-Sleep -Milliseconds 250
$plan = [ordered]@{
    inputMethod='Windows SendInput -> viewport child HWND WndProc'
    runId=$ready.RunId; processId=$ProcessId; mainWindowBounds=([Wp2NativeInput]::Bounds($window))
    viewportHwnd=$viewportHwnd.ToInt64(); viewportBounds=$bounds
    wheelPoint=@($wheelX,$wheelY); orbitStart=@($orbitX,$orbitY)
    wheelCount=$WheelCount; orbitSteps=$OrbitSteps; reverseWheelCount=$ReverseWheelCount
    startedAt=[DateTimeOffset]::UtcNow.ToString('O')
}
$plan | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'native-input.plan.json') -Encoding utf8

function Invoke-CapturedInput([string]$Stage, [scriptblock]$InputAction) {
    $command = Join-Path $OutputDirectory 'native-input.next.json'
    $armed = Join-Path $OutputDirectory ($Stage + '.armed')
    $complete = Join-Path $OutputDirectory ($Stage + '.complete')
    $wndProcLog = Join-Path $OutputDirectory 'native-input.log'
    $wndProcBefore = if (Test-Path -LiteralPath $wndProcLog) {
        @(Select-String -Path $wndProcLog -Pattern '\[NATIVE-WNDPROC\]').Count
    } else { 0 }
    $temporaryCommand = $command + '.tmp'
    @{ Stage=$Stage } | ConvertTo-Json | Set-Content -LiteralPath $temporaryCommand -Encoding utf8
    [IO.File]::Move($temporaryCommand, $command, $true)
    $stageTimer = [Diagnostics.Stopwatch]::StartNew()
    while (-not (Test-Path -LiteralPath $armed)) {
        if ($stageTimer.Elapsed.TotalSeconds -gt 30) { throw "Capture arm timeout: $Stage" }
        Start-Sleep -Milliseconds 20
    }
    & $InputAction
    while (-not (Test-Path -LiteralPath $complete)) {
        if ($timer.Elapsed.TotalSeconds -gt $TimeoutSeconds) { throw "Capture completion timeout: $Stage" }
        Start-Sleep -Milliseconds 20
    }
    $inputDeadline = [Diagnostics.Stopwatch]::StartNew()
    do {
        $wndProcAfter = if (Test-Path -LiteralPath $wndProcLog) {
            @(Select-String -Path $wndProcLog -Pattern '\[NATIVE-WNDPROC\]').Count
        } else { 0 }
        if ($wndProcAfter -gt $wndProcBefore) { return }
        Start-Sleep -Milliseconds 20
    } while ($inputDeadline.Elapsed.TotalSeconds -lt 5)
    throw "No native HWND WndProc event observed for $Stage."
}

for ($i = 0; $i -lt $WheelCount; $i++) {
    $stage = 'native-wheel-{0:D3}' -f $i
    Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(120) }
}

[Wp2NativeInput]::MoveTo($orbitX, $orbitY)
Start-Sleep -Milliseconds 100
Invoke-CapturedInput 'native-orbit-down' { [Wp2NativeInput]::Mouse(0x0020, 0) }
for ($i = 0; $i -lt $OrbitSteps; $i++) {
    $x = $orbitX + [int](($i + 1) * 12)
    $y = $orbitY - [int](($i + 1) * 6)
    $stage = 'native-orbit-move-{0:D2}' -f $i
    $move = { [Wp2NativeInput]::MoveTo($x, $y) }.GetNewClosure()
    Invoke-CapturedInput $stage $move
}
Invoke-CapturedInput 'native-orbit-up' { [Wp2NativeInput]::Mouse(0x0040, 0) }

[Wp2NativeInput]::MoveTo($wheelX, $wheelY)
for ($i = 0; $i -lt $ReverseWheelCount; $i++) {
    $stage = 'native-reverse-wheel-{0:D2}' -f $i
    Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(-120) }
}

Invoke-CapturedInput 'native-view-all' {
    [Wp2NativeInput]::MoveTo($bounds[0] + 190, $bounds[1] + 112)
    [Wp2NativeInput]::Mouse(0x0002, 0)
    [Wp2NativeInput]::Mouse(0x0004, 0)
}
Set-Content -LiteralPath (Join-Path $OutputDirectory 'native-input.stop') -Value 'STOP' -NoNewline
