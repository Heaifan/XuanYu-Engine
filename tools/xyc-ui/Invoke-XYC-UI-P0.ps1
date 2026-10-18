[CmdletBinding()]
param(
    [string]$AppPath = (Join-Path $PSScriptRoot '..\..\XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe'),
    [string]$EvidenceDirectory = ''
)
$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$cli = Join-Path $repo 'XuanYu.Editor.Control.Cli\bin\Debug\net10.0\XuanYu.Editor.Control.Cli.exe'
$AppPath = (Resolve-Path $AppPath).Path
$uiSource = Join-Path $repo 'XuanYu.Editor.UI\bin\Debug\net10.0\XuanYu.Editor.UI.dll'
$uiRuntime = Join-Path (Split-Path -Parent $AppPath) 'XuanYu.Editor.UI.dll'
if ((Get-FileHash $uiSource).Hash -ne (Get-FileHash $uiRuntime).Hash) {
    throw 'Editor App 未包含当前 scoped build 的 XuanYu.Editor.UI.dll；先构建 XuanYu.Editor.App。'
}
$runId = [guid]::NewGuid().ToString('N')
if ([string]::IsNullOrWhiteSpace($EvidenceDirectory)) {
    $EvidenceDirectory = Join-Path $repo "artifacts\diagnostics\xyc-ui-v0.1-p0\$runId"
}
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
New-Item -ItemType Directory -Force -Path $EvidenceDirectory | Out-Null
$script:requests = [Collections.Generic.List[object]]::new()
$start = [Diagnostics.ProcessStartInfo]::new($cli)
$start.Arguments = "--app `"$AppPath`" --xyc-ui --session-id $runId --timeout-seconds 300 --evidence-dir `"$EvidenceDirectory`""
$start.UseShellExecute = $false; $start.CreateNoWindow = $false
$start.RedirectStandardInput = $true; $start.RedirectStandardOutput = $true
$process = [Diagnostics.Process]::new(); $process.StartInfo = $start
$outputId = "XYC-UI-$runId"
$null = Register-ObjectEvent -InputObject $process -EventName OutputDataReceived -SourceIdentifier $outputId
$null = $process.Start(); $process.BeginOutputReadLine()

function Invoke-XYC([string]$command, [object]$argumentsObject = @{}) {
    $request = [ordered]@{ schemaVersion='XYC-API/1'; requestId=[guid]::NewGuid().ToString('N'); sessionId=$runId; command=$command; args=$argumentsObject }
    $script:requests.Add($request); $process.StandardInput.WriteLine(($request | ConvertTo-Json -Compress -Depth 20))
    $event = Wait-Event -SourceIdentifier $outputId -Timeout 60
    if ($null -eq $event) { throw "等待 $command 响应超时。" }
    $line = $event.SourceEventArgs.Data
    Remove-Event -EventIdentifier $event.EventIdentifier
    $response = $line | ConvertFrom-Json
    if (-not $response.success) { throw "$command 失败：$($response.errorCode)" }
    return $response.observedState
}
$captures = [Collections.Generic.List[object]]::new()
$runtime = $null; $initial = $null; $close = $null

try {
    $runtime = Invoke-XYC 'runtime.info'
    $initial = Invoke-XYC 'ui.window.inspect'
    foreach ($width in @(1360, 1920)) {
        $window = Invoke-XYC 'ui.window.resize' @{ widthDip=$width; heightDip=820 }
        if ([math]::Abs($window.widthDip - $width) -gt 1) { throw "窗口客户区宽度未达到 ${width} DIP：$($window.widthDip)" }
        foreach ($theme in @('Light', 'Dark')) {
            $null = Invoke-XYC 'ui.theme.set' @{ theme=$theme }
            $actualTheme = Invoke-XYC 'ui.theme.inspect'
            if ($actualTheme.actual -ne $theme) { throw "主题切换未生效：$theme/$($actualTheme.actual)" }
            $region = Invoke-XYC 'ui.region.inspect'
            $name = "$theme-$width.png"
            Add-Type -AssemblyName System.Drawing
            $pixelReady = $false
            for ($attempt=0; $attempt -lt 8 -and -not $pixelReady; $attempt++) {
                $capture = Invoke-XYC 'ui.capture' @{ fileName=$name }
                $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $capture.imagePath).Hash
                if ($hash -ne $capture.sha256) { throw "截图哈希不匹配：$name" }
                $bitmap = [Drawing.Bitmap]::new($capture.imagePath)
                if ($bitmap.Width -ne $capture.pixelWidth -or $bitmap.Height -ne $capture.pixelHeight) {
                    $bitmap.Dispose(); throw "PNG 尺寸与证据不符：$name"
                }
                # (3,3) is the rounded window corner and stays identical in both themes.
                # Sample a stable interior TOP surface pixel instead.
                $sample = $bitmap.GetPixel(100, 10)
                $bitmap.Dispose()
                $mean = ($sample.R + $sample.G + $sample.B) / 3
                $pixelReady = $theme -eq 'Light' ? ($mean -gt 170) : ($mean -lt 120)
            }
            $capture | Add-Member -NotePropertyName BackgroundSampleRgb -NotePropertyValue "$($sample.R),$($sample.G),$($sample.B)"
            $captures.Add($capture)
            if (-not $pixelReady) { throw "主题像素在 8 次实时捕获内未就绪：$name" }
        }
    }
    foreach ($width in @(1360, 1920)) {
        $light = $captures | Where-Object { [IO.Path]::GetFileName($_.imagePath) -eq "Light-$width.png" }
        $dark = $captures | Where-Object { [IO.Path]::GetFileName($_.imagePath) -eq "Dark-$width.png" }
        if ($light.BackgroundSampleRgb -eq $dark.BackgroundSampleRgb) {
            throw "Light/Dark 像素重复：$width"
        }
    }
    $close = Invoke-XYC 'session.close'
    if (-not $close.closed) { throw 'Session 未确认关闭。' }
    if (-not $process.WaitForExit(15000)) { throw 'Editor/CLI 会话未在 15 秒内退出。' }
    $manifest = [ordered]@{ status='RUNTIME_CAPTURED'; runId=$runId; runtime=$runtime; initialWindow=$initial; captures=$captures; requests=$script:requests; sessionClose=$close; cliExited=$process.HasExited; cliExitCode=$process.ExitCode; windowForegroundObserved=($captures | ForEach-Object { $_.window.foreground }); logs=@() }
    $manifest | ConvertTo-Json -Depth 30 | Set-Content -Encoding UTF8 (Join-Path $EvidenceDirectory 'runtime-evidence.json')
    Write-Output "XYC UI P0 runtime evidence: $EvidenceDirectory"
    Write-Output "Captures: $($captures.Count); CLI exit: $($process.ExitCode)"
} catch {
    $failure = $_.ToString()
    try { $close = Invoke-XYC 'session.close' } catch { }
    if (-not $process.HasExited) { $null = $process.WaitForExit(10000) }
    $manifest = [ordered]@{ status='FAILED'; runId=$runId; runtime=$runtime; initialWindow=$initial; captures=$captures; requests=$script:requests; sessionClose=$close; cliExited=$process.HasExited; cliExitCode=$(if ($process.HasExited) { $process.ExitCode } else { $null }); error=$failure; logs=@() }
    $manifest | ConvertTo-Json -Depth 30 | Set-Content -Encoding UTF8 (Join-Path $EvidenceDirectory 'runtime-evidence.json')
    throw
} finally {
    if (-not $process.HasExited) {
        try { $process.StandardInput.Close() } catch { }
        if (-not $process.WaitForExit(10000)) { $process.Kill($true); $process.WaitForExit() }
    }
    Remove-Event -SourceIdentifier $outputId -ErrorAction SilentlyContinue
    $process.Dispose()
}
