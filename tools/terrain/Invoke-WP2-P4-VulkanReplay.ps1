[CmdletBinding()]
param(
    [string]$TerrainPath = '',
    [string[]]$Variants = @('baseline', 'far-expanded', 'cull-off', 'lod0', 'near-small'),
    [double[]]$FarPlaneThresholds = @(),
    [int]$TimeoutSeconds = 240,
    [switch]$NativeInputReplay,
    [int]$NativeWheelCount = 40,
    [int]$NativeOrbitSteps = 8
)

function Get-TreeFingerprint([string]$base, [string[]]$paths) {
    $records = foreach ($path in ($paths | Sort-Object -Unique)) {
        $fullPath = Join-Path $base $path
        if (Test-Path -LiteralPath $fullPath -PathType Leaf) {
            $fileHash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash
            "$path|$fileHash"
        }
    }
    $bytes = [Text.Encoding]::UTF8.GetBytes(($records -join "`n"))
    $sha = [Security.Cryptography.SHA256]::Create()
    try { [BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-', '') }
    finally { $sha.Dispose() }
}

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$root = Join-Path $repo 'artifacts\diagnostics\wp2-p4-r3'
New-Item -ItemType Directory -Force -Path $root | Out-Null
if ([string]::IsNullOrWhiteSpace($TerrainPath)) {
    $TerrainPath = Join-Path $root 'n23e121.hgt'
    if (-not (Test-Path -LiteralPath $TerrainPath)) { & (Join-Path $PSScriptRoot 'New-WP2-P4-ReplayHgt.ps1') }
}
$TerrainPath = (Resolve-Path -LiteralPath $TerrainPath).Path
$terrainHash = (Get-FileHash -LiteralPath $TerrainPath -Algorithm SHA256).Hash
$head = (& git -C $repo rev-parse HEAD).Trim()
$dirty = if ((git -C $repo status --porcelain=v1).Count -gt 0) { 'YES' } else { 'NO' }
$stamp = [DateTimeOffset]::UtcNow.ToString('O')
$publish = Join-Path $root 'app'
New-Item -ItemType Directory -Force -Path $publish | Out-Null
$buildLog = Join-Path $root 'publish.log'
& (Join-Path $repo 'scripts\xye-dotnet.ps1') publish `
    (Join-Path $repo 'XuanYu.Editor.App\XuanYu.Editor.App.csproj') `
    --no-restore --configuration Release --runtime win-x64 --self-contained false `
    --output $publish *> $buildLog
if ($LASTEXITCODE -ne 0) { throw "Windows Vulkan replay publish failed; see $buildLog" }
$exe = Join-Path $publish 'XuanYu.Editor.App.exe'
if (-not (Test-Path -LiteralPath $exe)) { throw "Publish did not create $exe" }
$sourcePaths = @(& git -C $repo ls-files --cached --others --exclude-standard)
$sourceFingerprint = Get-TreeFingerprint $repo $sourcePaths
$publishedPaths = @(Get-ChildItem -LiteralPath $publish -Recurse -File |
    ForEach-Object { $_.FullName.Substring($publish.Length + 1) })
$publishFingerprint = Get-TreeFingerprint $publish $publishedPaths
$runSpecs = [Collections.Generic.List[object]]::new()
foreach ($variant in $Variants) {
    $runSpecs.Add([pscustomobject]@{ Name=$variant; Policy=$variant; Far=$null })
}
foreach ($threshold in $FarPlaneThresholds) {
    if (-not [double]::IsFinite($threshold) -or $threshold -le 0) {
        throw "FarPlane 阈值必须是有限正数：$threshold"
    }
    $label = $threshold.ToString('0.###', [Globalization.CultureInfo]::InvariantCulture)
    $runSpecs.Add([pscustomobject]@{ Name="far-$label"; Policy='baseline'; Far=$threshold })
}

Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class Wp2ReplayWindow
{
    [StructLayout(LayoutKind.Sequential)] struct Rect { public int L, T, R, B; }
    [DllImport("user32.dll", EntryPoint="GetWindowRect")]
    static extern bool ReadRect(IntPtr hwnd, out Rect rect);
    [DllImport("user32.dll")] public static extern bool PrintWindow(
        IntPtr hwnd, IntPtr hdc, uint flags);
    [DllImport("user32.dll")] public static extern bool AllowSetForegroundWindow(int processId);
    public static int[] Bounds(IntPtr hwnd) {
        if (!ReadRect(hwnd, out var r)) throw new InvalidOperationException("GetWindowRect failed");
        return new[] { r.L, r.T, r.R, r.B };
    }
}
'@

foreach ($spec in $runSpecs) {
    $variant = $spec.Policy
    $runName = $spec.Name
    $runId = [Guid]::NewGuid().ToString('N')
    $dir = Join-Path (Join-Path $root $runId) $runName
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $env:XYE_PROBE_VIEWPORT = '1'
    $env:XYE_PROBE_DIRECTORY = $dir
    $env:XYE_PROBE_SOURCE_HEAD = $head
    $env:XYE_PROBE_SOURCE_DIRTY = $dirty
    $env:XYE_PROBE_BUILD_TIMESTAMP = $stamp
    $env:XYE_PROBE_SOURCE_FINGERPRINT = $sourceFingerprint
    $env:XYE_PROBE_PUBLISH_FINGERPRINT = $publishFingerprint
    $env:XYE_TERRAIN_REPLAY = '1'
    $env:XYE_TERRAIN_AB = $variant
    if ($null -eq $spec.Far) { Remove-Item Env:XYE_TERRAIN_FAR_PLANE -ErrorAction SilentlyContinue }
    else { $env:XYE_TERRAIN_FAR_PLANE = $spec.Far.ToString('R', [Globalization.CultureInfo]::InvariantCulture) }
    $env:XYE_TERRAIN_REPLAY_HGT = $TerrainPath
    $env:XYE_TERRAIN_REPLAY_OUTPUT = $dir
    $env:XYE_TERRAIN_REPLAY_RUN_ID = $runId
    if ($NativeInputReplay) { $env:XYE_TERRAIN_NATIVE_INPUT_REPLAY = '1' }
    else { Remove-Item Env:XYE_TERRAIN_NATIVE_INPUT_REPLAY -ErrorAction SilentlyContinue }
    $stdout = Join-Path $dir 'stdout.log'
    $stderr = Join-Path $dir 'stderr.log'
    $process = Start-Process -FilePath $exe -WorkingDirectory $publish `
        -WindowStyle Normal -RedirectStandardOutput $stdout `
        -RedirectStandardError $stderr -PassThru
    $foregroundPermissionGranted = [Wp2ReplayWindow]::AllowSetForegroundWindow($process.Id)
    $watch = [Diagnostics.Stopwatch]::StartNew()
    $nativeJob = $null
    try {
        if ($NativeInputReplay) {
            $nativeJob = Start-Job -FilePath (Join-Path $PSScriptRoot 'Invoke-WP2-P4-NativeInputEvents.ps1') `
                -ArgumentList $dir, $process.Id, $NativeWheelCount, $NativeOrbitSteps, 3, $TimeoutSeconds
        }
        while (-not $process.HasExited) {
            $process.Refresh()
            if ($nativeJob -and $nativeJob.State -eq 'Failed') {
                throw "Native input driver failed: $(Receive-Job $nativeJob -Keep | Out-String)"
            }
            $replayLog = Join-Path $dir 'replay.log'
            $completionLog = if ($NativeInputReplay) { Join-Path $dir 'native-input.log' } else { $replayLog }
            $completionPattern = if ($NativeInputReplay) {
                '\[NATIVE-REPLAY-COMPLETE\]'
            } else { '\[REPLAY-COMPLETE\]' }
            if ((Test-Path -LiteralPath $completionLog) -and
                (Select-String -Path $completionLog -Pattern $completionPattern -Quiet)) {
                $process.WaitForExit(10000)
                if (-not $process.HasExited) {
                    throw 'Replay completed but editor process did not exit within 10 seconds.'
                }
                break
            }
            foreach ($requestFile in Get-ChildItem -LiteralPath $dir -Filter '*.capture.request.json') {
                $ack = $requestFile.FullName + '.ack'
                if (Test-Path -LiteralPath $ack) { continue }
                try {
                    $request = Get-Content -LiteralPath $requestFile.FullName -Raw | ConvertFrom-Json
                    if ($request.ProcessId -ne $process.Id) { throw 'Capture request process id mismatch.' }
                    if ($process.MainWindowHandle -eq [IntPtr]::Zero) { throw 'Main window handle unavailable.' }
                    $targetWindow = $process.MainWindowHandle
                    $bounds = [Wp2ReplayWindow]::Bounds($process.MainWindowHandle)
                    $width = $bounds[2] - $bounds[0]; $height = $bounds[3] - $bounds[1]
                    if ($width -le 0 -or $height -le 0) { throw 'Window bounds are empty.' }
                    $bitmap = [Drawing.Bitmap]::new($width, $height)
                    $graphics = [Drawing.Graphics]::FromImage($bitmap)
                    try {
                        $hdc = $graphics.GetHdc()
                        try {
                            if (-not [Wp2ReplayWindow]::PrintWindow($targetWindow, $hdc, 2)) {
                                throw 'PrintWindow could not capture the target Vulkan window.'
                            }
                        } finally { $graphics.ReleaseHdc($hdc) }
                        $bitmap.Save($request.ScreenshotPath, [Drawing.Imaging.ImageFormat]::Png)
                    } finally { $graphics.Dispose(); $bitmap.Dispose() }
                    $imageHash = (Get-FileHash -LiteralPath $request.ScreenshotPath -Algorithm SHA256).Hash
                    $capture = [ordered]@{ captureType='Windows PrintWindow target'; runId=$runId
                        variant=$variant; farPlaneOverride=$request.FarPlaneOverride
                        gpuFrameId=$request.FrameId
                        nativeInputEventId=$request.NativeInputEventId
                        viewportFrameId=$request.ViewportFrameId; processId=$process.Id
                        windowHandle=$targetWindow.ToInt64()
                        buildIdentity=$request.BuildIdentity; cameraPosition=$request.CameraPosition
                        sourceTreeFingerprint=$request.SourceTreeFingerprint
                        publishedOutputFingerprint=$request.PublishedOutputFingerprint
                        cameraForward=$request.CameraForward; fov=$request.Fov
                        sourceNearPlane=$request.NearPlane; sourceFarPlane=$request.FarPlane
                        effectiveNearPlane=$request.EffectiveNearPlane
                        effectiveFarPlane=$request.EffectiveFarPlane
                        cameraRevision=$request.CameraRevision
                        observationCenter=$request.ObservationCenter
                        renderOrigin=$request.RenderOrigin
                        terrainId=$request.TerrainId; terrainRevision=$request.TerrainRevision
                        physicalViewport=@($request.PhysicalWidth,$request.PhysicalHeight)
                        windowBounds=$bounds; physicalImageSize=@($width,$height)
                        terrainPath=$TerrainPath; terrainSha256=$terrainHash
                        imagePath=$request.ScreenshotPath; imageSha256=$imageHash
                        capturedAt=[DateTimeOffset]::UtcNow.ToString('O') }
                    $capture | ConvertTo-Json -Depth 4 | Set-Content `
                        -LiteralPath ($request.ScreenshotPath + '.json') -Encoding utf8
                    $ackTemp = $ack + '.tmp'
                    Set-Content -LiteralPath $ackTemp -Value 'CAPTURED' -NoNewline
                    [IO.File]::Move($ackTemp, $ack, $true)
                } catch {
                    $ackTemp = $ack + '.tmp'
                    Set-Content -LiteralPath $ackTemp -Value ("FAILED: " + $_.Exception.Message) -NoNewline
                    [IO.File]::Move($ackTemp, $ack, $true)
                }
            }
            if ($watch.Elapsed.TotalSeconds -gt $TimeoutSeconds) {
                throw "Replay timed out after $TimeoutSeconds seconds ($runName)."
            }
            Start-Sleep -Milliseconds 50
        }
        $process.WaitForExit()
        $replayLog = Join-Path $dir 'replay.log'
        $completionLog = if ($NativeInputReplay) { Join-Path $dir 'native-input.log' } else { $replayLog }
        $completionPattern = if ($NativeInputReplay) {
            '\[NATIVE-REPLAY-COMPLETE\]'
        } else { '\[REPLAY-COMPLETE\]' }
        if (-not (Test-Path $completionLog) -or -not (Select-String -Path $completionLog -Pattern $completionPattern -Quiet)) {
            throw "Replay did not complete for $runName; inspect $replayLog and stderr.log"
        }
        if ($nativeJob -and $nativeJob.State -eq 'Failed') {
            throw "Native input driver failed: $(Receive-Job $nativeJob -Keep | Out-String)"
        }
        [ordered]@{ variant=$runName; policyVariant=$variant; farPlaneThreshold=$spec.Far
            runId=$runId; sourceHead=$head
            foregroundPermissionGranted=$foregroundPermissionGranted
            sourceDirty=$dirty; buildTimeUtc=$stamp; terrainPath=$TerrainPath
            terrainSha256=$terrainHash; sourceTreeFingerprint=$sourceFingerprint
            publishedOutputFingerprint=$publishFingerprint
            exitCode=$process.ExitCode; status='COMPLETE' } |
            ConvertTo-Json | Set-Content -LiteralPath (Join-Path $dir 'run.manifest.json') -Encoding utf8
    } finally {
        if ($nativeJob -and $nativeJob.State -in @('Running','NotStarted')) { Stop-Job $nativeJob }
        if ($nativeJob) { Remove-Job $nativeJob -Force }
        if (-not $process.HasExited) { $process.Kill(); $process.WaitForExit() }
        $process.Dispose()
    }
}
Write-Output "Replay output: $root"
Write-Output "Terrain SHA256: $terrainHash"
