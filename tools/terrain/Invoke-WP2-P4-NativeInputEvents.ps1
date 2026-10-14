[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][int]$ProcessId,
    [string]$TargetScaleLabel = '2 km',
    [int]$MaxWheelSteps = 60,
    [int]$MaxPostScaleWheelSteps = 20,
    [ValidateSet('zoom-and-orbit','orbit-only')][string]$ReplayMode = 'zoom-and-orbit',
    [int]$OrbitSteps = 8,
    [int]$ReverseWheelCount = 3,
    [int]$TimeoutSeconds = 600
)

$ErrorActionPreference = 'Stop'
$OrbitOnly = $ReplayMode -eq 'orbit-only'
if ($TargetScaleLabel -ne '2 km') { throw 'R7 requires the exact visible scale label "2 km".' }
if ($OrbitSteps -lt 8 -or $OrbitSteps -gt 16) { throw 'OrbitSteps must be between 8 and 16.' }
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
  [DllImport("user32.dll", CharSet=CharSet.Unicode)] static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder text, int count);
  [DllImport("user32.dll", CharSet=CharSet.Unicode)] static extern int GetClassName(IntPtr hwnd, System.Text.StringBuilder text, int count);
  [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hwnd);
  [DllImport("user32.dll")] static extern bool AttachThreadInput(uint attach, uint attachTo, bool value);
  [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
  public static int[] Bounds(IntPtr hwnd) {
    if (!GetWindowRect(hwnd, out var r)) throw new InvalidOperationException("GetWindowRect failed");
    return new[] { r.left, r.top, r.right, r.bottom };
  }
  public static IntPtr FindViewport(IntPtr parent, int ownerProcessId, out int[] bounds) {
    IntPtr selected = IntPtr.Zero; var matches = 0; int[] selectedBounds = null;
    EnumChildWindows(parent, (child, _) => {
      if (!GetWindowRect(child, out var r)) return true;
      GetWindowThreadProcessId(child, out var childProcessId);
      if (childProcessId != ownerProcessId) return true;
      var className = new System.Text.StringBuilder(128); GetClassName(child, className, className.Capacity);
      if (className.ToString() == "XuanYuVulkanViewport" && IsWindowVisible(child)) {
        matches++; selected=child;
        selectedBounds=new[] { r.left,r.top,r.right,r.bottom };
      }
      return true;
    }, IntPtr.Zero);
    if (matches != 1)
      throw new InvalidOperationException("Expected one visible process-owned XuanYuVulkanViewport HWND; matches=" + matches);
    bounds=selectedBounds; return selected;
  }
  public static string ForegroundInfo() {
    var hwnd=GetForegroundWindow(); GetWindowThreadProcessId(hwnd, out var pid);
    var title=new System.Text.StringBuilder(512); GetWindowText(hwnd, title, title.Capacity);
    return "HWND=" + hwnd + ";PID=" + pid + ";Title=" + title;
  }
  public static bool IsForeground(IntPtr hwnd) => GetForegroundWindow() == hwnd;
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
    var deadline = Environment.TickCount64 + 60000;
    while (GetForegroundWindow() != hwnd && Environment.TickCount64 < deadline) {
      SetForegroundWindow(hwnd);
      System.Threading.Thread.Sleep(250);
    }
    var active = GetForegroundWindow();
    if (active != hwnd) throw new InvalidOperationException("Replay window is not foreground; " + ForegroundInfo());
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
$viewportHwnd = [Wp2NativeInput]::FindViewport($window, $ProcessId, [ref]$bounds)
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
    viewportNativePhysical=@($width,$height)
    viewportReportedPhysical=@($ready.Viewport.PhysicalWidth,$ready.Viewport.PhysicalHeight)
    viewportDpiScale=$ready.Viewport.DpiScale
    wheelPoint=@($wheelX,$wheelY); orbitStart=@($orbitX,$orbitY)
    targetScaleLabel=$TargetScaleLabel; maxWheelSteps=$MaxWheelSteps
    maxPostScaleWheelSteps=$MaxPostScaleWheelSteps; orbitSteps=$OrbitSteps
    replayMode=$ReplayMode
    reverseWheelCount=$ReverseWheelCount
    startedAt=[DateTimeOffset]::UtcNow.ToString('O')
}
$plan | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'native-input.plan.json') -Encoding utf8

function Get-NativeLines([string]$Path) {
    if (Test-Path -LiteralPath $Path) { return @(Get-Content -LiteralPath $Path) }
    return @()
}

function Invoke-CapturedInput([string]$Stage, [scriptblock]$InputAction,
    [string]$ExpectedMessage, [string]$ExpectedGesture) {
    $command = Join-Path $OutputDirectory 'native-input.next.json'
    $armed = Join-Path $OutputDirectory ($Stage + '.armed')
    $inputComplete = Join-Path $OutputDirectory ($Stage + '.input-complete')
    $complete = Join-Path $OutputDirectory ($Stage + '.complete')
    $wndProcLog = Join-Path $OutputDirectory 'native-input.log'
    $gestureLog = Join-Path $OutputDirectory 'input-camera.log'
    $wndProcBefore = @(Get-NativeLines $wndProcLog).Count
    $gestureBefore = @(Get-NativeLines $gestureLog).Count
    $temporaryCommand = $command + '.tmp'
    @{ Stage=$Stage } | ConvertTo-Json | Set-Content -LiteralPath $temporaryCommand -Encoding utf8
    [IO.File]::Move($temporaryCommand, $command, $true)
    $stageTimer = [Diagnostics.Stopwatch]::StartNew()
    while (-not (Test-Path -LiteralPath $armed)) {
        if ($stageTimer.Elapsed.TotalSeconds -gt 30) { throw "Capture arm timeout: $Stage" }
        Start-Sleep -Milliseconds 20
    }
    if ($null -ne $InputAction) {
        if (-not [Wp2NativeInput]::IsForeground($window)) {
            throw "Replay target lost foreground before $Stage; $([Wp2NativeInput]::ForegroundInfo())"
        }
        & $InputAction
        if (-not [string]::IsNullOrWhiteSpace($ExpectedMessage)) {
            $inputDeadline = [Diagnostics.Stopwatch]::StartNew()
            do {
                $newLines = @(Get-NativeLines $wndProcLog | Select-Object -Skip $wndProcBefore)
                $messagePattern = 'Message=' + [regex]::Escape($ExpectedMessage) + ';Hwnd=' +
                    [regex]::Escape($viewportHwnd.ToInt64().ToString()) + ';'
                if ($newLines | Where-Object { $_ -match '\[NATIVE-WNDPROC\]' -and $_ -match $messagePattern }) {
                    if ($ExpectedGesture) {
                        $gestureLines = @(Get-NativeLines $gestureLog | Select-Object -Skip $gestureBefore)
                        $gesturePattern = '\[NATIVE-GESTURE\] Phase=' + [regex]::Escape($ExpectedGesture) + ';Owner=Camera;'
                        if (-not ($gestureLines | Where-Object { $_ -match $gesturePattern })) {
                            Start-Sleep -Milliseconds 20; continue
                        }
                    }
                    break
                }
                Start-Sleep -Milliseconds 20
            } while ($inputDeadline.Elapsed.TotalSeconds -lt 5)
            if ($inputDeadline.Elapsed.TotalSeconds -ge 5) {
                throw "Expected native HWND event/Camera gesture not observed for $Stage."
            }
        }
    }
    Set-Content -LiteralPath $inputComplete -Value 'INPUT_CONFIRMED' -NoNewline
    while (-not (Test-Path -LiteralPath $complete)) {
        if ($timer.Elapsed.TotalSeconds -gt $TimeoutSeconds) { throw "Capture completion timeout: $Stage" }
        Start-Sleep -Milliseconds 20
    }
}

function Get-Capture([string]$Stage) {
    $capture = Get-Content -LiteralPath (Join-Path $OutputDirectory ($Stage + '.capture.request.json')) -Raw | ConvertFrom-Json
    return $capture
}

Invoke-CapturedInput 'PRE_WHEEL' $null $null $null
$capture = Get-Capture 'PRE_WHEEL'
$scaleLog = [Collections.Generic.List[object]]::new()
$scaleLog.Add([pscustomobject]@{ Stage='PRE_WHEEL'; Scale=$capture.ScaleBar; CameraDistanceMeters=$capture.ScaleBar.CameraToPivotMeters })
$targetReached = $capture.ScaleBar.Text -ceq $TargetScaleLabel
$wheelStepsToScale = 0
for ($i = 0; $i -lt $MaxWheelSteps -and -not $targetReached; $i++) {
    $stage = 'native-wheel-{0:D3}' -f $i
    Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(120) } '0x20A' $null
    $capture = Get-Capture $stage
    $scaleLog.Add([pscustomobject]@{ Stage=$stage; Scale=$capture.ScaleBar; CameraDistanceMeters=$capture.ScaleBar.CameraToPivotMeters })
    $wheelStepsToScale++
    $targetReached = $capture.ScaleBar.Text -ceq $TargetScaleLabel
}

if ($targetReached) {
    Invoke-CapturedInput 'PRE_2KM' $null $null $null
    $pre2Km = Get-Capture 'PRE_2KM'
    $scaleRestored = $OrbitOnly
    if (-not $OrbitOnly) {
        $postScaleSteps = 0
        for ($i = 0; $i -lt $MaxPostScaleWheelSteps; $i++) {
            $stage = 'post-2km-wheel-{0:D2}' -f $i
            Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(120) } '0x20A' $null
            $capture = Get-Capture $stage
            $scaleLog.Add([pscustomobject]@{ Stage=$stage; Scale=$capture.ScaleBar; CameraDistanceMeters=$capture.ScaleBar.CameraToPivotMeters })
            $postScaleSteps++
        }
        for ($i = 0; $i -lt $postScaleSteps -and -not $scaleRestored; $i++) {
            $stage = 'restore-2km-wheel-{0:D2}' -f $i
            Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(-120) } '0x20A' $null
            $capture = Get-Capture $stage
            $scaleLog.Add([pscustomobject]@{ Stage=$stage; Scale=$capture.ScaleBar; CameraDistanceMeters=$capture.ScaleBar.CameraToPivotMeters })
            if ($capture.ScaleBar.Text -ceq $TargetScaleLabel) { $scaleRestored = $true }
        }
    }
    if (-not $scaleRestored) { throw 'After reverse wheel, UI scale label did not return to 2 km.' }
    Invoke-CapturedInput 'PRE_ORBIT' $null $null $null
    $preOrbit = Get-Capture 'PRE_ORBIT'
    $orbitStartRevision = $preOrbit.CameraRevision
    [Wp2NativeInput]::MoveTo($orbitX, $orbitY)
    Start-Sleep -Milliseconds 100
    Invoke-CapturedInput 'native-orbit-down' { [Wp2NativeInput]::Mouse(0x0020, 0) } '0x207' 'Begin'
    for ($i = 0; $i -lt $OrbitSteps; $i++) {
        $x = $orbitX + [int](($i + 1) * 12)
        $y = $orbitY - [int](($i + 1) * 6)
        $stage = 'native-orbit-move-{0:D2}' -f $i
        $move = { [Wp2NativeInput]::MoveTo($x, $y) }.GetNewClosure()
        Invoke-CapturedInput $stage $move '0x200' 'Update'
    }
    Invoke-CapturedInput 'POST_ORBIT' { [Wp2NativeInput]::Mouse(0x0040, 0) } '0x208' 'Commit'
    $postOrbit = Get-Content (Join-Path $OutputDirectory 'POST_ORBIT.capture.request.json') -Raw | ConvertFrom-Json
    if ($postOrbit.CameraRevision -le $orbitStartRevision) { throw 'Camera revision did not change during Orbit.' }
    $pivotDelta = [Math]::Sqrt(
        [Math]::Pow([double]$postOrbit.ObservationCenter.X - [double]$preOrbit.ObservationCenter.X, 2) +
        [Math]::Pow([double]$postOrbit.ObservationCenter.Y - [double]$preOrbit.ObservationCenter.Y, 2) +
        [Math]::Pow([double]$postOrbit.ObservationCenter.Z - [double]$preOrbit.ObservationCenter.Z, 2))
    if ($pivotDelta -gt 1e-9) { throw "Frozen Pivot moved during Orbit: $pivotDelta meters." }

    [Wp2NativeInput]::MoveTo($orbitX + $OrbitSteps * 12, $orbitY - $OrbitSteps * 6)
    Invoke-CapturedInput 'reverse-orbit-down' { [Wp2NativeInput]::Mouse(0x0020, 0) } '0x207' 'Begin'
    for ($i = 0; $i -lt $OrbitSteps; $i++) {
        $x = $orbitX + [int](($OrbitSteps - $i - 1) * 12)
        $y = $orbitY - [int](($OrbitSteps - $i - 1) * 6)
        $stage = 'native-reverse-orbit-{0:D2}' -f $i
        $move = { [Wp2NativeInput]::MoveTo($x, $y) }.GetNewClosure()
        Invoke-CapturedInput $stage $move '0x200' 'Update'
    }
    Invoke-CapturedInput 'reverse-orbit-up' { [Wp2NativeInput]::Mouse(0x0040, 0) } '0x208' 'Commit'
}

[Wp2NativeInput]::MoveTo($wheelX, $wheelY)
for ($i = 0; $i -lt $ReverseWheelCount; $i++) {
    $stage = 'native-reverse-wheel-{0:D2}' -f $i
    Invoke-CapturedInput $stage { [Wp2NativeInput]::Wheel(-120) } '0x20A' $null
}

$outcome = [ordered]@{ scaleTargetReached=$targetReached; targetScaleLabel=$TargetScaleLabel
    wheelStepsToScale=$wheelStepsToScale; pre2KmScale=$pre2Km.ScaleBar
    preOrbitScale=$preOrbit.ScaleBar; preOrbitCameraDistanceMeters=$preOrbit.ScaleBar.CameraToPivotMeters
    orbitOnly=$OrbitOnly; orbitExecuted=$targetReached; orbitSteps=if ($targetReached) {$OrbitSteps} else {0}; scaleHistory=@($scaleLog)
    result=if ($targetReached) {'SCALEBAR_2KM_NATIVE_REPLAY_BLUR_REVIEW_REQUIRED'} else {'NOT_REPRODUCED_SCALEBAR_2KM_NOT_REACHED'} }
$outcome | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'native-input.outcome.json') -Encoding utf8
Set-Content -LiteralPath (Join-Path $OutputDirectory 'native-input.stop') -Value 'STOP' -NoNewline
