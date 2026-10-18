[CmdletBinding()]
param(
    [string]$AppPath = (Join-Path $PSScriptRoot '..\..\XuanYu.Editor.App\bin\Release\net10.0\XuanYu.Editor.App.exe'),
    [string]$EvidenceDirectory = '',
    [string]$CandidateIdentity = 'UNSPECIFIED'
)
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$cli = Join-Path $repo 'XuanYu.Editor.Control.Cli\bin\Release\net10.0\XuanYu.Editor.Control.Cli.exe'
$AppPath = (Resolve-Path $AppPath).Path
$runId = [guid]::NewGuid().ToString('N')
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) {
    $EvidenceDirectory = Join-Path $repo "docs\qa\XYC-UI-V0.1-P1-A\evidence\$runId"
}
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Force -Path $EvidenceDirectory | Out-Null
$runs = [Collections.Generic.List[object]]::new()

function Invoke-Case([int]$width, [string]$theme) {
    $session = [guid]::NewGuid().ToString('N')
    $caseDir = Join-Path $EvidenceDirectory "$theme-$width"
    New-Item -ItemType Directory -Force -Path $caseDir | Out-Null
    $start = [Diagnostics.ProcessStartInfo]::new($cli)
    $start.Arguments = "--app `"$AppPath`" --xyc-ui --diagnostic-write --session-id $session --timeout-seconds 300 --evidence-dir `"$caseDir`""
    $start.UseShellExecute = $false; $start.RedirectStandardInput = $true
    $start.RedirectStandardOutput = $true; $start.RedirectStandardError = $false
    $start.Environment['XYC_BUILD_IDENTITY'] = $CandidateIdentity
    $process = [Diagnostics.Process]::new(); $process.StartInfo = $start
    $outputId = "XYC-P1A-$session"
    $null = Register-ObjectEvent -InputObject $process -EventName OutputDataReceived -SourceIdentifier $outputId
    $requests = [Collections.Generic.List[object]]::new()
    $process.Start() | Out-Null; $process.BeginOutputReadLine()
    function Send([string]$command, [object]$payload = @{}, [string]$sessionId = $session) {
        $request = [ordered]@{ schemaVersion='XYC-API/1'; requestId=[guid]::NewGuid().ToString('N'); sessionId=$sessionId; command=$command; args=$payload }
        $requests.Add($request); $process.StandardInput.WriteLine(($request | ConvertTo-Json -Compress -Depth 24))
        $event = Wait-Event -SourceIdentifier $outputId -Timeout 90
        if ($null -eq $event) { throw "等待 $command 响应超时。" }
        $line = $event.SourceEventArgs.Data; Remove-Event -EventIdentifier $event.EventIdentifier
        return $line | ConvertFrom-Json
    }
    try {
        $runtime = (Send 'runtime.info').observedState
        $window = (Send 'ui.window.resize' @{ widthDip=$width; heightDip=820 }).observedState
        if ($runtime.processId -ne $window.processId -or [string]::IsNullOrWhiteSpace($window.hwnd)) { throw 'Editor PID/HWND 所有权核验失败。' }
        if ($window.widthDip -ne $width -or $window.heightDip -ne 820) { throw "窗口尺寸回读不符：$($window.widthDip)x$($window.heightDip)" }
        $actualTheme = (Send 'ui.theme.set' @{ theme=$theme }).observedState
        if ($actualTheme.actual -ne $theme) { throw "主题回读不符：$($actualTheme.actual)" }
        $probe = (Send 'ui.capture' @{ fileName="$theme-$width-probe.png"; backend='avaloniaWindow' }).observedState
        if ($probe.theme.actual -ne $theme -or $probe.pixelWidth -le 0 -or $probe.pixelHeight -le 0) {
            throw "$theme 主题未通过真实 Window 捕获状态核验。"
        }
        $controls = (Send 'ui.control.list').observedState
        $controlListPath = Join-Path $caseDir 'control-list.json'
        $controls | ConvertTo-Json -Depth 16 | Set-Content -Encoding UTF8 $controlListPath
        $targets = @('XYE.TOP.VIEW.ALL','XYE.TOP.VIEW.PAN','XYE.TOP.VIEW.ORBIT',
            'XYE.TOP.EDIT.SELECT','XYE.TOP.EDIT.BOX_SELECT','XYE.TOP.FILE.SAVE')
        $targetStates = foreach ($id in $targets) {
            $matches = @($controls | Where-Object { $_.controlId -eq $id })
            if ($matches.Count -ne 1 -or $matches[0].duplicateId) { throw "控件 ID 缺失或重复：$id" }
            if ($matches[0].idSource -notin @('XYDiagnostic.DebugId','AutomationId','Name')) { throw "控件 ID 来源无效：$id" }
            $matches[0]
        }
        $save = (Send 'ui.control.inspect' @{ controlId='XYE.TOP.FILE.SAVE' }).observedState
        $box = (Send 'ui.control.inspect' @{ controlId='XYE.TOP.EDIT.BOX_SELECT' }).observedState
        $before = (Send 'ui.state.inspect').observedState
        $originalCamera = (Send 'camera.inspect').observedState
        $testPose = [ordered]@{ position=@{ x=($originalCamera.position.x+500); y=($originalCamera.position.y+700); z=($originalCamera.position.z+250) };
            forward=$originalCamera.forward; up=$originalCamera.up; fovDegrees=$originalCamera.fovDegrees;
            nearPlane=$originalCamera.nearPlane; farPlane=$originalCamera.farPlane; mode=$originalCamera.mode;
            orthographicScale=$originalCamera.orthographicScale; observationCenter=$originalCamera.observationCenter;
            revision=$originalCamera.revision }
        $seeded = Send 'camera.set-test-pose' $testPose
        if (-not $seeded.success) { throw "测试会话准备姿态失败：$($seeded.errorCode)" }
        $seedState = $seeded.observedState.after
        $null = Send 'ui.waitUntil' @{ state='cameraRevision'; expected=$seedState.revision; timeoutMs=5000 }
        $negative = [ordered]@{}
        $negative.missing = (Send 'ui.control.inspect' @{ controlId='XYE.TOP.MISSING' }).errorCode -eq 'CONTROL_NOT_FOUND'
        $negative.disabled = (Send 'ui.control.invoke' @{ controlId='XYE.TOP.EDIT.BOX_SELECT'; operation='invoke' }).errorCode -eq 'CONTROL_DISABLED'
        $negative.disallowedSave = (Send 'ui.control.invoke' @{ controlId='XYE.TOP.FILE.SAVE'; operation='invoke' }).errorCode -eq 'OPERATION_NOT_ALLOWED'
        $negative.wrongSession = (Send 'ui.control.list' @{} 'not-this-session').errorCode -eq 'UNAUTHORIZED'
        $negative.timeout = (Send 'ui.waitUntil' @{ state='cameraRevision'; expected=($before.cameraRevision + 999999); timeoutMs=50 }).errorCode -eq 'TIMEOUT'
        if ($negative.Values -contains $false) { throw "生产 Runtime 负向测试失败：$($negative | ConvertTo-Json -Compress)" }
        $invoke = (Send 'ui.control.invoke' @{ controlId='XYE.TOP.VIEW.ALL'; operation='invoke' })
        if (-not $invoke.success) { throw "AutomationPeer 调用失败：$($invoke.errorCode)" }
        $effect = $invoke.observedState
        $positionDelta = [math]::Abs($effect.after.cameraX-$effect.before.cameraX) +
            [math]::Abs($effect.after.cameraY-$effect.before.cameraY) + [math]::Abs($effect.after.cameraZ-$effect.before.cameraZ)
        if (-not $effect.invoked -or $effect.after.cameraRevision -le $effect.before.cameraRevision -or $positionDelta -lt 1) {
            throw 'VIEW.ALL 的实际相机位置没有发生可证变化。'
        }
        $settled = (Send 'ui.waitUntil' @{ state='cameraRevision'; expected=$effect.after.cameraRevision; timeoutMs=5000 })
        if (-not $settled.success) { throw "操作后状态未就绪：$($settled.errorCode)" }
        $capture = (Send 'ui.capture' @{ fileName="$theme-$width.png"; backend='avaloniaWindow' })
        if (-not $capture.success) { throw "TOP 截图失败：$($capture.errorCode)" }
        $restored = Send 'camera.set-test-pose' $originalCamera
        if (-not $restored.success) { throw "测试会话相机恢复失败：$($restored.errorCode)" }
        $restoredCamera = (Send 'camera.inspect').observedState
        if ([math]::Abs($restoredCamera.position.x-$originalCamera.position.x) -gt 0.001 -or
            [math]::Abs($restoredCamera.position.y-$originalCamera.position.y) -gt 0.001 -or
            [math]::Abs($restoredCamera.position.z-$originalCamera.position.z) -gt 0.001) { throw '测试会话没有恢复初始相机位置。' }
        $close = (Send 'session.close')
        if (-not $close.success -or -not $process.WaitForExit(15000) -or $process.ExitCode -ne 0) { throw 'Session 未正常退出。' }
        $runs.Add([ordered]@{ widthDip=$width; heightDip=820; theme=$theme; processId=$runtime.processId;
            hwnd=$window.hwnd; window=$window; themeState=$actualTheme; controls=$targetStates;
            controlListPath=$controlListPath; controlCount=@($controls).Count;
            saveInspect=$save; disabledInspect=$box; originalCamera=$originalCamera; seededCamera=$seedState;
            before=$before; invoke=$effect; restoredCamera=$restoredCamera; themePixelSample="$($sample.R),$($sample.G),$($sample.B)";
            wait=$settled.observedState; capture=$capture.observedState; negative=$negative;
            sessionClose=$close.observedState; cliExitCode=$process.ExitCode; requests=$requests })
    } finally {
        if (-not $process.HasExited) { try { $process.StandardInput.Close() } catch {}; if (-not $process.WaitForExit(8000)) { $process.Kill($true); $process.WaitForExit() } }
        Remove-Event -SourceIdentifier $outputId -ErrorAction SilentlyContinue
        $process.Dispose()
    }
}

try {
    foreach ($width in @(1360,1920)) { foreach ($theme in @('Light','Dark')) { Invoke-Case $width $theme } }
    $manifest = [ordered]@{ status='RUNTIME_PASS'; runId=$runId; candidateIdentity=$CandidateIdentity;
        appSha256=(Get-FileHash -Algorithm SHA256 -LiteralPath $AppPath).Hash;
        uiSha256=(Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path (Split-Path $AppPath) 'XuanYu.Editor.UI.dll')).Hash;
        runs=$runs }
    $manifest | ConvertTo-Json -Depth 32 | Set-Content -Encoding UTF8 (Join-Path $EvidenceDirectory 'runtime-evidence.json')
    Write-Output "P1-A Runtime PASS: $EvidenceDirectory"
} catch {
    $manifest = [ordered]@{ status='RUNTIME_FAIL'; runId=$runId; candidateIdentity=$CandidateIdentity; runs=$runs; error=$_.ToString() }
    $manifest | ConvertTo-Json -Depth 32 | Set-Content -Encoding UTF8 (Join-Path $EvidenceDirectory 'runtime-evidence.json')
    throw
}
