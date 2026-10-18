$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$app = Join-Path $repo 'XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe'
$cli = Join-Path $repo 'XuanYu.Editor.Control.Cli\bin\Debug\net10.0\XuanYu.Editor.Control.Cli.exe'
$id = [guid]::NewGuid().ToString('N')
$start = [Diagnostics.ProcessStartInfo]::new($cli)
$start.UseShellExecute = $false; $start.RedirectStandardInput = $true
$start.RedirectStandardOutput = $true; $start.RedirectStandardError = $true
foreach ($arg in @('--app', $app, '--session-id', $id, '--xyc-silent', '--timeout-seconds', '90')) {
    $start.ArgumentList.Add($arg)
}
$process = [Diagnostics.Process]::Start($start)
$stderr = $process.StandardError.ReadToEndAsync()
$appPid = 0
$result = [ordered]@{ status='RUNNING'; sessionId=$id; checks=[ordered]@{} }
function Send-Xyc([string]$command) {
    $request = @{ schemaVersion='XYC-API/1'; requestId=[guid]::NewGuid().ToString('N'); sessionId=$id; command=$command; args=@{} } | ConvertTo-Json -Compress
    $process.StandardInput.WriteLine($request)
    $task = $process.StandardOutput.ReadLineAsync()
    if (-not $task.Wait([TimeSpan]::FromSeconds(45))) { throw "$command timed out" }
    return $task.Result | ConvertFrom-Json
}
try {
    $runtime = $null; $scene = $null
    for ($i=0; $i -lt 30; $i++) {
        $runtime = (Send-Xyc 'runtime.info').observedState
        $scene = (Send-Xyc 'scene.inspect').observedState
        if ($runtime.silentHost.presentSucceeded -and $scene.presentedFrameId) { break }
        Start-Sleep -Milliseconds 300
    }
    $appPid = $runtime.processId; $hostState = $runtime.silentHost
    $result.runtime = $runtime; $result.scene = $scene
    $result.checks['silentIsolationMode'] = $runtime.isolationMode -eq 'silent-hidden-hwnd'
    $result.checks['hiddenNativeWindow'] = $hostState.hwndExists -and -not $hostState.visible -and -not $hostState.wsVisibleStyle
    $result.checks['foregroundNotTaken'] = -not $hostState.ownProcessForeground
    $result.checks['fixedViewport'] = $hostState.clientWidth -eq 776 -and $hostState.clientHeight -eq 650
    $result.checks['presentSucceeded'] = $hostState.presentSucceeded -and $scene.presentedFrameId -gt 0
    $closed = Send-Xyc 'session.close'
    $result.checks['sessionCloseAccepted'] = $closed.success -and $closed.observedState.closed
    $process.StandardInput.Close()
    $result.checks['cleanProcessExit'] = $process.WaitForExit(30000) -and $process.ExitCode -eq 0
    $result.checks['noEditorProcessResidue'] = -not (Get-Process -Id $appPid -ErrorAction SilentlyContinue)
    $result.status = if ($result.checks.Values -contains $false) { 'FAIL' } else { 'PASS' }
} catch {
    $result.status = 'FAIL'; $result.error = $_.ToString()
} finally {
    if (-not $process.HasExited) { $process.Kill($true); $process.WaitForExit() }
    $result.monitorLog = $stderr.GetAwaiter().GetResult()
    $out = Join-Path $repo 'artifacts\diagnostics\xyc-ui-v0.1-p0\silent-runtime-evidence.json'
    $result | ConvertTo-Json -Depth 16 | Set-Content -Encoding UTF8 $out
    $process.Dispose()
}
$result | ConvertTo-Json -Depth 16
if ($result.status -ne 'PASS') { exit 1 }
