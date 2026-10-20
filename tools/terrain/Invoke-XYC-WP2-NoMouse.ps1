param(
    [string]$CliPath = $(if ($env:XYE_BUILD_DIRECTORY) { Join-Path $env:XYE_BUILD_DIRECTORY 'bin\XuanYu.Editor.Control.Cli\debug\XuanYu.Editor.Control.Cli.exe' } else { Join-Path $PSScriptRoot '..\..\XuanYu.Editor.Control.Cli\bin\Debug\net10.0\XuanYu.Editor.Control.Cli.exe' }),
    [string]$AppPath = $(if ($env:XYE_BUILD_DIRECTORY) { Join-Path $env:XYE_BUILD_DIRECTORY 'bin\XuanYu.Editor.App\debug\XuanYu.Editor.App.exe' } else { Join-Path $PSScriptRoot '..\..\XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe' }),
    [string]$OutputRoot = $(if ($env:XYE_DIAGNOSTICS_DIRECTORY) { $env:XYE_DIAGNOSTICS_DIRECTORY } else { Join-Path $PSScriptRoot '..\..\artifacts\diagnostics\xyc-r0-a1' })
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$workspaceOutput = [IO.Path]::GetFullPath($OutputRoot)
if ([string]::IsNullOrWhiteSpace($env:XYE_TASK_RUN_ID) -or
    [string]::IsNullOrWhiteSpace($env:XYE_DIAGNOSTICS_DIRECTORY) -or
    [string]::IsNullOrWhiteSpace($env:XYE_BUILD_DIRECTORY)) {
    throw 'Run XYC WP2 through scripts\xye-workspace.ps1.'
}
$cli = [IO.Path]::GetFullPath($CliPath)
$app = [IO.Path]::GetFullPath($AppPath)
$outputBase = [IO.Path]::GetFullPath($OutputRoot)
$dem = Join-Path $root 'artifacts\diagnostics\wp2-p4-r3\n24e121.hgt'
$expectedHash = 'BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1'
& (Join-Path $root 'scripts\xye-dotnet.ps1') build (Join-Path $root 'XuanYu.Editor.Control.Cli\XuanYu.Editor.Control.Cli.csproj') --no-restore -m 1 -nr false -p BuildInParallel=false -p UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'XYC CLI/App build failed.' }
if (-not (Test-Path -LiteralPath $cli) -or -not (Test-Path -LiteralPath $app)) { throw 'Workspace CLI or Editor App output is missing.' }
if ((Get-FileHash -LiteralPath $dem -Algorithm SHA256).Hash -ne $expectedHash) { throw 'Registered NASADEM HGT hash mismatch.' }
$allowed = [IO.Path]::GetFullPath($env:XYE_DIAGNOSTICS_DIRECTORY)
if (-not $outputBase.Equals($allowed, [StringComparison]::OrdinalIgnoreCase)) { throw "OutputRoot must equal the isolated XYC diagnostics directory: $allowed" }
$runId = [Guid]::NewGuid().ToString('N')
$runDir = Join-Path $outputBase $runId
New-Item -ItemType Directory -Force -Path $runDir | Out-Null
$branch = if ($env:XYE_SOURCE_BRANCH) { $env:XYE_SOURCE_BRANCH } else { (& git -C $root branch --show-current).Trim() }
$head = if ($env:XYE_SOURCE_HEAD) { $env:XYE_SOURCE_HEAD } else { (& git -C $root rev-parse HEAD).Trim() }
Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class XycWindowCapture {
    delegate bool EnumProc(IntPtr hwnd, IntPtr state);
    [StructLayout(LayoutKind.Sequential)] struct Rect { public int L,T,R,B; }
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc callback, IntPtr state);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
    [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
    public static IntPtr FindVisibleWindow(int processId) {
        IntPtr found = IntPtr.Zero;
        EnumWindows((hwnd, state) => { uint pid; GetWindowThreadProcessId(hwnd, out pid);
            if (pid == processId && IsWindowVisible(hwnd)) { found = hwnd; return false; } return true; }, IntPtr.Zero);
        return found;
    }
    public static int[] Bounds(IntPtr hwnd) {
        Rect r; if (!GetWindowRect(hwnd, out r)) throw new InvalidOperationException("GetWindowRect failed.");
        return new[] { r.L, r.T, r.R, r.B };
    }
}
'@
$sessionId = "xyc-r0-a1-$runId"
$start = [Diagnostics.ProcessStartInfo]::new($cli)
$start.UseShellExecute = $false
$start.RedirectStandardInput = $true
$start.RedirectStandardOutput = $true
$start.RedirectStandardError = $true
foreach ($pair in @(@('--app',$app),@('--session-id',$sessionId),@('--diagnostic-write'),@('--test-dem',$dem),@('--timeout-seconds','180'))) {
    foreach ($value in $pair) { $start.ArgumentList.Add($value) }
}
$start.Environment['XYC_BRANCH'] = $branch
$start.Environment['XYE_PROBE_VIEWPORT'] = '1'
$start.Environment['XYE_TERRAIN_REPLAY'] = '1'
$start.Environment['XYE_PROBE_DIRECTORY'] = $runDir
$start.Environment['XYE_PROBE_SOURCE_HEAD'] = $head
$start.Environment['XYE_PROBE_SOURCE_DIRTY'] = 'YES'
$start.Environment['XYE_PROBE_BUILD_TIMESTAMP'] = (Get-Date).ToUniversalTime().ToString('O')
$process = [Diagnostics.Process]::Start($start)
if ($null -eq $process) { throw 'Could not start isolated Editor process.' }
$stderr = $process.StandardError.ReadToEndAsync()
$requestCounter = 0
$commandTrace = [Collections.Generic.List[object]]::new()
function Send-Xyc([string]$command, [object]$arguments) {
    $script:requestCounter++
    $request = @{ schemaVersion='XYC-API/1'; requestId="r$script:requestCounter"; sessionId=$sessionId; command=$command; args=$arguments }
    $requestedAt = [DateTimeOffset]::UtcNow
    $process.StandardInput.WriteLine(($request | ConvertTo-Json -Depth 10 -Compress))
    $line = $process.StandardOutput.ReadLine()
    if ([string]::IsNullOrWhiteSpace($line)) { throw "No XYC response for $command." }
    $acknowledgedAt = [DateTimeOffset]::UtcNow
    $response = $line | ConvertFrom-Json
    if (-not $response.success) { throw "XYC $command failed: $($response.errorCode)" }
    $script:commandTrace.Add([ordered]@{ requestId=$request.requestId; command=$command;
        requestedAtUtc=$requestedAt.ToString('O'); acknowledgedAtUtc=$acknowledgedAt.ToString('O') })
    return $response
}
function Wait-XycPose([string]$label, [long]$cameraRevision, [long]$afterPresentedFrame) {
    $clock = [Diagnostics.Stopwatch]::StartNew()
    do {
        $scene = Send-Xyc 'scene.inspect' @{}
        if ($scene.observedState.scaleLabel -eq $label -and
            $scene.observedState.presentedFrameId -gt $afterPresentedFrame -and
            $scene.observedState.presentedCameraRevision -eq $cameraRevision -and
            $scene.observedState.presentedCaptureHeld -eq $true) { return $scene }
        # Polling cadence only limits IPC volume; completion requires the matching Presented CameraRevision.
        Start-Sleep -Milliseconds 50
    } while ($clock.Elapsed.TotalSeconds -lt 30)
    throw "Timed out waiting for presented CameraRevision=$cameraRevision and $label. Last=$($scene | ConvertTo-Json -Depth 10 -Compress)"
}
function Save-WindowedEvidence([string]$stage, $runtime, $scene, $camera, [string]$cameraCommandRequestId) {
    if ($scene.observedState.presentedCameraRevision -ne $camera.cameraRevision) {
        throw "Presented camera revision mismatch at $stage."
    }
    if ($scene.observedState.presentedCaptureHeld -ne $true) { throw "Presented frame was not held during capture at $stage." }
    $captureStartedAt = [DateTimeOffset]::UtcNow
    $hwnd = [XycWindowCapture]::FindVisibleWindow([int]$runtime.observedState.processId)
    if ($hwnd -eq [IntPtr]::Zero) { throw 'No visible top-level Editor HWND found.' }
    $bounds = [XycWindowCapture]::Bounds($hwnd)
    $width = $bounds[2] - $bounds[0]; $height = $bounds[3] - $bounds[1]
    if ($width -le 0 -or $height -le 0) { throw 'Editor HWND has invalid bounds.' }
    $imagePath = Join-Path $runDir "$stage.png"
    $bitmap = [Drawing.Bitmap]::new($width, $height)
    $graphics = [Drawing.Graphics]::FromImage($bitmap)
    $hdc = $graphics.GetHdc()
    try { if (-not [XycWindowCapture]::PrintWindow($hwnd,$hdc,2)) { throw 'Windows PrintWindow failed.' } }
    finally { $graphics.ReleaseHdc($hdc); $graphics.Dispose() }
    $bitmap.Save($imagePath,[Drawing.Imaging.ImageFormat]::Png); $bitmap.Dispose()
    $captureCompletedAt = [DateTimeOffset]::UtcNow
    $record = [ordered]@{ stage=$stage; classification='PARTIAL / WINDOWED'; imageSource='Windows PrintWindow';
        image=$imagePath; imageSha256=(Get-FileHash -LiteralPath $imagePath -Algorithm SHA256).Hash;
        captureStartedAtUtc=$captureStartedAt.ToString('O'); captureCompletedAtUtc=$captureCompletedAt.ToString('O');
        cameraCommandRequestId=$cameraCommandRequestId;
        buildIdentity=$runtime.observedState.build; processId=$runtime.observedState.processId;
        sceneId=$scene.observedState.sceneId; sceneRevision=$scene.sceneRevision;
        terrainId=$scene.observedState.terrainId; terrainRevision=$scene.observedState.terrainRevision;
        scaleLabel=$scene.observedState.scaleLabel; viewportFrameId=$scene.observedState.viewportFrameId;
        presentedFrameId=$scene.observedState.presentedFrameId;
        renderSnapshotCameraRevision=$scene.observedState.presentedCameraRevision;
        presentedImageIndex=$scene.observedState.presentedImageIndex;
        presentedCaptureHeld=$scene.observedState.presentedCaptureHeld;
        viewportWidth=$scene.observedState.viewportWidth; viewportHeight=$scene.observedState.viewportHeight;
        cameraRevision=$camera.cameraRevision; camera=$camera.observedState }
    $record | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath (Join-Path $runDir "$stage.json") -Encoding utf8
    return $record
}
function Capture-PresentedStage([string]$stage, [string]$label, $runtime, $camera, $cameraCommand, [long]$afterFrame) {
    $null = Send-Xyc 'capture.arm' @{cameraRevision=$camera.observedState.revision}
    try {
        $scene = Wait-XycPose $label $camera.observedState.revision $afterFrame
        $record = Save-WindowedEvidence $stage $runtime $scene $camera $cameraCommand.requestId
        return [pscustomobject]@{ Scene=$scene; Record=$record }
    }
    finally { $null = Send-Xyc 'capture.release' @{} }
}
try {
    $runtime = Send-Xyc 'runtime.info' @{}
    $scene0 = Send-Xyc 'scene.inspect' @{}
    if ($scene0.observedState.terrainId -ne 'n24e121') { throw "Wrong terrain identity: $($scene0.observedState.terrainId)" }
    if ($scene0.observedState.presentedFrameId -le 0) { throw 'No presented Vulkan frame observed.' }
    $pose2 = @{ position=@{x=67003.16836731856;y=35216.59496208729;z=14165.543022747646};
        forward=@{x=-0.5656854249492377;y=0.7071067811865475;z=-0.4242640687119291};
        up=@{x=-0.265035662579632;y=0.33129457822454017;z=0.9055385138137415};
        fovDegrees=60.0;nearPlane=0.05;farPlane=102709.47646704617;mode='Perspective';orthographicScale=0.0;
        observationCenter=@{x=50648.444336988396;y=55660.0;z=1899.5};revision=0 }
    $pose20 = @{ position=@{
            x=$pose2.observationCenter.x + 10 * ($pose2.position.x - $pose2.observationCenter.x)
            y=$pose2.observationCenter.y + 10 * ($pose2.position.y - $pose2.observationCenter.y)
            z=$pose2.observationCenter.z + 10 * ($pose2.position.z - $pose2.observationCenter.z) };
        forward=@{x=$pose2.forward.x;y=$pose2.forward.y;z=$pose2.forward.z};
        up=@{x=$pose2.up.x;y=$pose2.up.y;z=$pose2.up.z};
        fovDegrees=$pose2.fovDegrees;nearPlane=$pose2.nearPlane;farPlane=1000000.0;
        mode=$pose2.mode;orthographicScale=$pose2.orthographicScale;
        observationCenter=@{x=$pose2.observationCenter.x;y=$pose2.observationCenter.y;z=$pose2.observationCenter.z};
        revision=0 }
    $set20Initial = Send-Xyc 'camera.set-test-pose' $pose20
    $pose20InitialRead = Send-Xyc 'camera.inspect' @{}
    $initial20 = Capture-PresentedStage 'pose-20km-initial' '20 km' $runtime $pose20InitialRead $set20Initial ([long]$scene0.observedState.presentedFrameId)
    $set2 = Send-Xyc 'camera.set-test-pose' $pose2
    $pose2Read = Send-Xyc 'camera.inspect' @{}
    $stage2 = Capture-PresentedStage 'pose-2km' '2 km' $runtime $pose2Read $set2 ([long]$initial20.Scene.observedState.presentedFrameId)
    $orbit = Send-Xyc 'camera.orbit' @{deltaX=45.0;deltaY=25.0}
    $orbitCamera = Send-Xyc 'camera.inspect' @{}
    $stageOrbit = Capture-PresentedStage 'editor-orbit-2km' '2 km' $runtime $orbitCamera $orbit ([long]$stage2.Scene.observedState.presentedFrameId)
    $set20 = Send-Xyc 'camera.set-test-pose' $pose20
    $pose20Read = Send-Xyc 'camera.inspect' @{}
    $stage20 = Capture-PresentedStage 'pose-20km' '20 km' $runtime $pose20Read $set20 ([long]$stageOrbit.Scene.observedState.presentedFrameId)
    $gpuCaptures = foreach ($stage in @($initial20,$stage2,$stageOrbit,$stage20)) {
        $frameId = [long]$stage.Scene.observedState.presentedFrameId
        $revision = [long]$stage.Scene.observedState.presentedCameraRevision
        $gpuPath = Join-Path $runDir ('gpu-frame-{0:D8}-camera-{1:D8}.png' -f $frameId,$revision)
        $metadataPath = [IO.Path]::ChangeExtension($gpuPath,'.json')
        if (-not (Test-Path -LiteralPath $gpuPath) -or -not (Test-Path -LiteralPath $metadataPath)) {
            throw "GPU readback missing for FrameId=$frameId CameraRevision=$revision."
        }
        $gpuMetadata = Get-Content -LiteralPath $metadataPath -Raw | ConvertFrom-Json
        $gpuHash = (Get-FileHash -LiteralPath $gpuPath -Algorithm SHA256).Hash
        if ($gpuMetadata.frameId -ne $frameId -or $gpuMetadata.cameraRevision -ne $revision -or
            $gpuMetadata.source -ne 'VulkanSwapchainImage-GPU-Readback' -or $gpuMetadata.sha256 -ne $gpuHash) {
            throw "GPU readback provenance/hash mismatch for FrameId=$frameId CameraRevision=$revision."
        }
        if ([string]::IsNullOrWhiteSpace($gpuMetadata.device) -or $gpuMetadata.device -eq 'UNKNOWN') {
            throw "GPU device identity missing for FrameId=$frameId CameraRevision=$revision."
        }
        [ordered]@{stage=$stage.Record.stage;frameId=$frameId;cameraRevision=$revision;
            image=$gpuPath;imageSha256=$gpuHash;format=$gpuMetadata.format;
            source=$gpuMetadata.source;device=$gpuMetadata.device;buildIdentity=$gpuMetadata.build;
            sceneId=$stage.Record.sceneId;sceneRevision=$stage.Record.sceneRevision;
            terrainId=$stage.Record.terrainId;terrainRevision=$stage.Record.terrainRevision}
    }
    if ($gpuCaptures[1].imageSha256 -eq $gpuCaptures[2].imageSha256) {
        throw 'The 2 km and Orbit GPU readbacks are identical.'
    }
    $null = Send-Xyc 'session.close' @{}
    $process.WaitForExit(10000) | Out-Null
    [ordered]@{ runId=$runId; classification='GPU READBACK PASS / WINDOWED'; offscreen='NOT RUN';
        mouseKeyboardForegroundInjection='NONE'; sourceHgt=$dem; sourceSha256=$expectedHash;
        gpuReadbacks=$gpuCaptures;
        startupScene=$scene0.observedState; initial20km=$initial20.Record; pose2km=$stage2.Record;
        editorOrbit2km=$stageOrbit.Record; pose20km=$stage20.Record; commandTrace=$commandTrace;
        commands=@('runtime.info','scene.inspect','camera.inspect','camera.set-test-pose','camera.orbit','capture.arm','capture.release','session.close') } |
        ConvertTo-Json -Depth 24 | Set-Content -LiteralPath (Join-Path $runDir 'run-manifest.json') -Encoding utf8
    Write-Output "XYC GPU READBACK PASS / WINDOWED; TRUE OFFSCREEN NOT RUN: $runDir"
    Write-Output "2 km GPU PNG SHA256: $($gpuCaptures[1].imageSha256)"
    Write-Output "Orbit GPU PNG SHA256: $($gpuCaptures[2].imageSha256)"
    Write-Output "20 km GPU PNG SHA256: $($gpuCaptures[3].imageSha256)"
}
finally {
    if (-not $process.HasExited) { $process.Kill($true); $process.WaitForExit() }
}
